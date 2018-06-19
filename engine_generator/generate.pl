#!/usr/bin/perl

use strict;
use warnings;

use lib qw(lib);
use Mustache::Simple;

my $table_file = 'engine_tables.txt';

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

open my $info, $table_file or die "Could not open $table_file: $!";

my $categories = {
	"basic" => [],
	"exotics" => []
};

my $header = <$info>;
while (my $line = <$info>)  {
	my @cols = split(' ', $line);
	my $rating = $cols[0];

	my $category = "ignore";

	if ($rating == 60) {
	} elsif ($rating < 100) {
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

		write_to_file("../data/$category/engines/$engine->{ID}.json", $json);
		my $engines = $categories->{$category};
		push(@$engines, $engine);
	};

	if ($rating == 60) {
		$category = "basic";
	}

	if ($rating == 60 || $rating % 25 == 0) {
		$category = "basic";
	} else {
		#$category = "full";
		next;
	}
	
	$generate_engine_sub->("emod_engine_std", $cols[5], 5000);

	if ($rating % 100 == 0) {
		$category = "basic";
	} else {
		#$category = "full";
		next;
	}

	$generate_engine_sub->("emod_engine_xl", $cols[7], 20000);

	if ($rating % 100 == 0) {
		$category = "exotics";
	} else {
		#$category = "full";
		next;
	}

	$generate_engine_sub->("emod_engine_light", $cols[6], 10000);
	$generate_engine_sub->("emod_engine_compact", $cols[4], 5000);
	$generate_engine_sub->("emod_engine_cxl", $cols[7], 30000);
	$generate_engine_sub->("emod_engine_xxl", $cols[8], 25000);
	$generate_engine_sub->("emod_engine_cxxl", $cols[8], 40000);
}

close $info;

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
