#!/usr/bin/perl

use strict;
use warnings;

use lib qw(lib);
use List::Util qw[min max];
use POSIX;
use Mustache::Simple;

my $table_file = 'engine_tables.txt';

my $tache = new Mustache::Simple(
	throw => 1
);

open my $handle, '<', "icons.txt";
chomp(my @icons = <$handle>);
close $handle;

my %stockratings;
{
	my @ratings = get_lines_from_file("stock_std_ratings.txt");
	push(@ratings, get_lines_from_file("lore_ratings.txt"));
	push(@ratings, get_lines_from_file("special_ratings.txt"));
	@stockratings{@ratings} = ();
}

my $icon = "uixSvgIcon_equipment_Heatsink";
# useful to browse icons
sub next_icon {
	#push(@icons, shift(@icons));
	#$icon = $icons[0];
	return $icon;
}

open my $info, $table_file or die "Could not open $table_file: $!";

my $categories = {
	"basic" => [],
	"exotics" => []
};

my @overview_rows = ();

my $header = <$info>;
while (my $line = <$info>)  {
	my @cols = split(' ', $line);
	
	my $rating = $cols[0];
	my $std_tonnage = $cols[5];
	my $light_tonnage = $cols[6];
	my $xl_tonnage = $cols[7];
	my $xxl_tonnage = $cols[8];

	my $category = "basic";
	
	#next unless (exists $stockratings{$rating});

	my $rating_string = sprintf('%03s', $rating);
	print($rating_string, " ");
	my $gyro_tons = ceil($rating / 100);
	my $gyro_cost = 300000 * $gyro_tons;
	my $heat_dissipation = min(floor($rating / 25), 10) * 3;
	my $additional_slots = max(floor($rating / 25 - 10), 0);

	my $hs_free = 10;
	my $total = $rating / 25;
	my $ihs_count = int(min($hs_free, $total));
	my $ehs_count = int(max(0, $hs_free - $ihs_count));
	my $ahs_count = int(max(0, $total - $hs_free));
	
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
		
		my $engine = {
			ID => "${prefix}_${rating_string}",
			RATING => $rating,
			RATING_STRING => $rating_string,
			TONNAGE => $std_tonnage + $gyro_tons,
			COST => $engine_cost + $gyro_cost,
			ICON => next_icon(),
			BONUSA => "- ${heat_dissipation} Heat / Turn",
			BONUSB => $additional_slots == 0 ? " " : "+ $additional_slots Slots"
		};

		my $json = $tache->render("${prefix}_template.json", $engine);

		write_to_file("../data/$category/engines/$engine->{ID}.json", $json);
		my $engines = $categories->{$category};
		push(@$engines, $engine);
	};
	
	$generate_engine_sub->("emod_engine");
}

close $info;

{
	my $json = $tache->render("overview.html.mustache", { "rows" => \@overview_rows });
	write_to_file("overview.html", $json);
}

while ((my $category, my $engines) = each(%{$categories})) {
	my $shop = {
		ID => "shopdef_emod_engines_${category}_test_generated",
		ENGINES => $engines
	};

	my $json = $tache->render('shopdef_emod_engines_template.json', $shop);
	write_to_file("../data/${category}/shops_test/$shop->{ID}.json", $json);
}

sub write_to_file {
	my $filename = shift;
	my $content = shift;
	open(my $fh, '>', $filename) or die "Could not open file '$filename' $!";
	print {$fh} $content;
	close $fh;
}

sub get_lines_from_file {
	local $/ = "\r\n";
	my $filename = shift;
	open my $handle, '<', $filename;
	chomp(my @lines = <$handle>);
	close $handle;
	return @lines;
}
