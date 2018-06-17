#!/usr/bin/perl

use strict;
use warnings;

use lib qw(lib);
use Mustache::Simple;

my $table_file = 'engine_tables.txt';

my $tache = new Mustache::Simple(
	throw => 1
);

my $engine_base_dir = '../engines';
my $shops_base_dir = '../shops';

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

my $header = <$info>;
while (my $line = <$info>)  {
	my @cols = split(' ', $line);
	my $rating = $cols[0];
	if ($rating == 60) {

	} elsif ($rating < 100) {
		next;
	} elsif ($rating % 50 != 0) {
		next;
	}
	
	my $rating_string = sprintf('%03s', $rating);
	print($rating_string, " ");
	my $gyro_tons = int($rating / 100 + 0.5);
	my $gyro_cost = 300000 * $gyro_tons;

	{
		my $engine = {
			ID => "emod_engine_std_$rating_string",
			RATING => $rating_string,
			TONNAGE => $cols[5] + $gyro_tons,
			COST => 5000 * $rating + $gyro_cost, # we assume 75 ton mech
			ICON => next_icon()
		}; 

		my $json = $tache->render('emod_engine_std_template.json', $engine);
		write_to_file("$engine_base_dir/$engine->{ID}.json", $json);

		push(@ENGINES, $engine);
	}

	if ($rating % 100 != 0) {
		next;
	}

	{
		my $engine = {
			ID => "emod_engine_xl_$rating_string",
			RATING => $rating_string,
			TONNAGE => $cols[7] + $gyro_tons,
			COST => 20000 * $rating + $gyro_cost, # we assume 75 ton mech
			ICON => next_icon()
		};

		my $json = $tache->render('emod_engine_xl_template.json', $engine);
		write_to_file("$engine_base_dir/$engine->{ID}.json", $json);

		push(@ENGINES, $engine);
	}
}

close $info;

{
	my $shop = {
		ENGINES => \@ENGINES
	};

	my $json = $tache->render('shopdef_emod_test_generated.json', $shop);
	write_to_file("$shops_base_dir/shopdef_emod_test_generated.json", $json);
}

sub write_to_file {
	my $filename = shift;
	my $content = shift;
	open(my $fh, '>', $filename) or die "Could not open file '$filename' $!";
	print {$fh} $content;
	close $fh;
}
