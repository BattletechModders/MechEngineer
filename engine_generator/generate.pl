#!/usr/bin/perl

use strict;
use warnings;

use lib qw(lib);
use List::Util qw[min max];
use POSIX;
use Mustache::Simple;

local $/ = "\n";

my @engine_tonnages = get_table('engine_tables.txt');
my %engine_values = map { $_->[0] => $_->[1] } get_table('engine_values.txt');

my %stockratings;
{
	my @ratings = get_lines_from_file("stock_std_ratings.txt");
	#push(@ratings, get_lines_from_file("lore_ratings.txt"));
	push(@ratings, get_lines_from_file("special_ratings.txt"));
	@stockratings{@ratings} = ();
}

my $tache = new Mustache::Simple(
	throw => 1
);

open my $handle, '<', "icons.txt";
chomp(my @icons = <$handle>);
close $handle;

my $icon = "uixSvgIcon_equipment_Heatsink";
# useful to browse icons
sub next_icon {
	#push(@icons, shift(@icons));
	#$icon = $icons[0];
	return $icon;
}

my @overview_rows = ();

foreach my $row_ref (@engine_tonnages) {
	my @row = @{$row_ref};
	my $rating = $row[0];
	my $std_tonnage = $row[5];
	my $light_tonnage = $row[6];
	my $xl_tonnage = $row[7];
	my $xxl_tonnage = $row[8];

	my $rating_string = sprintf('%03s', $rating);
	print($rating_string, " ");
	my $gyro_tons = ceil($rating / 100);
	my $gyro_cost = 300000 * $gyro_tons;

	my $hs_free = 10;
	my $total = floor($rating / 25);
	
	my $ihs_count = min($hs_free, $total);
	my $ehs_count = max(0, $hs_free - $ihs_count);
	my $ahs_count = max(0, $total - $hs_free);
	
	my $hs_free_tonnage = $ehs_count * 1;
	my $heat_dissipation = min($total, 10) * 3;
	
	my $tag = exists $stockratings{$rating} ? "component_type_stock" : "component_type_variant";
	
	push(@overview_rows, {
			rating => $rating,
			std_tonnage => $std_tonnage,
			light_tonnage => $light_tonnage,
			xl_tonnage => $xl_tonnage,
			gyro_tons => $gyro_tons,
			ihs_count => $ihs_count,
			ehs_count => $ehs_count,
			ahs_count => $ahs_count
		});
	
	my $generate_engine_sub = sub {
		my $prefix = shift;
		
		my $engine_cost = int($rating * $rating * $rating * $rating / 10000 / 10000) * 10000;
		
		my $total_cost = $engine_values{$rating};
		
		my $bonus = "";
		if ($ahs_count > 0) {
			$bonus = "\"EngineHSCap: $ahs_count\""
		}
		if ($hs_free_tonnage > 0) {
			$bonus = "\"EngineHSFreeExt: $hs_free_tonnage\""
		}

		my $engine = {
			ID => "${prefix}_${rating_string}",
			RATING => $rating,
			RATING_STRING => $rating_string,
			TONNAGE => $std_tonnage + $gyro_tons,
			COST => $total_cost,
			ICON => next_icon(),
			BONUS => $bonus,
			TAG => $tag,
			IHS_COUNT => $ihs_count,
            AHS_COUNT => $ahs_count
		};

		my $json = $tache->render("${prefix}_template.json", $engine);

		write_to_file("../data/basic/engines/$engine->{ID}.json", $json);
	};
	
	$generate_engine_sub->("emod_engine");
}

{
	my $json = $tache->render("overview.html.mustache", { "rows" => \@overview_rows });
	write_to_file("overview.html", $json);
}

sub write_to_file {
	my $filename = shift;
	my $content = shift;
	open(my $fh, '>', $filename) or die "Could not open file '$filename' $!";
	print {$fh} $content;
	close $fh;
}

sub get_lines_from_file {
	my $filename = shift;
	open my $handle, '<', $filename;
	chomp(my @lines = <$handle>);
	close $handle;
	return @lines;
}

sub get_table {
	my $filename = shift;
	my @lines = get_lines_from_file($filename);
	shift(@lines); # header
	my @rows = ();
	for my $line (@lines) {
		chomp($line);
		my @row = split(/\s+/, $line);
		push(@rows, \@row);
	}
	return @rows;
}
