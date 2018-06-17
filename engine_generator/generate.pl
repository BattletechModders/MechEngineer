#!/usr/bin/perl

use strict;
use warnings;

use lib qw(lib);
use Mustache::Simple;

my $table_file = 'engine_tables.txt';

my $tache = new Mustache::Simple(
	throw => 1
);

my $engine_base_dir = '../engine_basic';
my $engine_more_dir = '../engine_more';
my $shops_base_dir = '../engine_basic_shops_test';
my $shops_more_dir = '../engine_more_shops_test';

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

open my $info, $table_file or die "Could not open $table_file: $!";

my @ENGINES = ();
my @MORE_ENGINES = ();

my $header = <$info>;
while (my $line = <$info>)  {
	my @cols = split(' ', $line);
	my $rating = $cols[0];

	my $more = 0;

	if ($rating == 60) {
	} elsif ($rating < 100) {
		next;
	} elsif ($rating % 25 != 0) {
		next;
	}

	my $rating_string = sprintf('%03s', $rating);
	print($rating_string, " ");
	my $gyro_tons = int($rating / 100 + 0.5);
	my $gyro_cost = 300000 * $gyro_tons;

	my $generate_engine_sub = sub {
		my $prefix = shift;
		my $engine_tonnage = shift;
		my $engine_cost_per_rating = shift;

		my $engine = {
			ID => "${prefix}_${rating_string}",
			RATING => $rating_string,
			TONNAGE => $engine_tonnage + $gyro_tons,
			COST => $engine_cost_per_rating * $rating + $gyro_cost, # we assume 75 ton mech
			ICON => next_icon()
		};

		my $json = $tache->render("${prefix}_template.json", $engine);

		if ($more) {
			write_to_file("$engine_more_dir/$engine->{ID}.json", $json);
			push(@MORE_ENGINES, $engine);
		} else {
			write_to_file("$engine_base_dir/$engine->{ID}.json", $json);
			push(@ENGINES, $engine);
		}
	};

	if ($rating % 50 != 0) {
		$more = 1;
	}

	if ($rating == 60) {
		$more = 0;
	}

	$generate_engine_sub->("emod_engine_std", $cols[5], 5000);

	if ($rating % 100 != 0) {
		$more = 1;
	}

	$generate_engine_sub->("emod_engine_xl", $cols[7], 20000);

	$more = 1;

	$generate_engine_sub->("emod_engine_cxl", $cols[7], 30000);
	$generate_engine_sub->("emod_engine_light", $cols[6], 10000);
	$generate_engine_sub->("emod_engine_compact", $cols[4], 5000);
	$generate_engine_sub->("emod_engine_xxl", $cols[8], 25000);
	$generate_engine_sub->("emod_engine_cxxl", $cols[8], 40000);
	$generate_engine_sub->("emod_engine_xl", $cols[7], 20000);
}

close $info;

{
	my $shop = {
		ID => "shopdef_emod_engines_test_generated",
		ENGINES => \@ENGINES
	};

	my $json = $tache->render('shopdef_emod_engines_template.json', $shop);
	write_to_file("$shops_base_dir/$shop->{ID}.json", $json);
}

{
	my $shop = {
		ID => "shopdef_emod_engines_more_test_generated",
		ENGINES => \@ENGINES
	};

	my $json = $tache->render('shopdef_emod_engines_template.json', $shop);
	write_to_file("$shops_more_dir/$shop->{ID}.json", $json);
}

sub write_to_file {
	my $filename = shift;
	my $content = shift;
	open(my $fh, '>', $filename) or die "Could not open file '$filename' $!";
	print {$fh} $content;
	close $fh;
}
