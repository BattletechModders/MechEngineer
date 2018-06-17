#!/usr/bin/perl

use strict;
use warnings;

my $table_file = 'engine_tables.txt';
my $std_template = slurp('emod_engine_std_template.json');
my $xl_template = slurp('emod_engine_xl_template.json');
my $light_template = slurp('emod_engine_light_template.json');
my $cxl_template = slurp('emod_engine_cxl_template.json');
my $compact_template = slurp('emod_engine_compact_template.json');
my $engine_base_dir = '../engines';

open my $info, $table_file or die "Could not open $table_file: $!";

my $header = <$info>;
while (my $line = <$info>)  {
	my @cols = split(' ', $line);
	my $rating = $cols[0];
	if ($rating == 60) {
	
	} elsif ($rating < 100) {
		next;
	} elsif ($rating % 25 != 0) {
		next;
	}
	
	my $gyro_tons = int($rating / 100 + 0.5);
	my $std_tons = $cols[5] + $gyro_tons;
	my $compact_tons = $cols[4] + $gyro_tons;
	
	my $gyro_cost = 300000 * $gyro_tons;
	my $std_cost = 5000 * $rating + $gyro_cost; # we assume 75 ton mech
	my $compact_cost = 5000 * $rating + $gyro_cost;
	
	my $rating_string = sprintf('%03s', $rating);
	print($rating_string, " ");
	
	if ($rating % 50 != 0) {
		next;
	}
	
	my $compact = $compact_template;
	$compact =~ s/\{\{RATING}}/$rating_string/g;
	$compact =~ s/\{\{TONNAGE}}/$compact_tons/g;
	$compact =~ s/\{\{COST}}/$compact_cost/g;
	write_to_file("$engine_base_dir/emod_engine_compact_$rating_string.json", $compact);

}

close $info;

sub slurp {
	my $filename = shift;
    my $content;
    open(my $fh, '<', $filename) or die "cannot open file $filename";
    {
        local $/;
        $content = <$fh>;
    }
    close($fh);
	return $content;
}

sub write_to_file {
	my $filename = shift;
	my $content = shift;
	open(my $fh, '>', $filename) or die "Could not open file '$filename' $!";
	print {$fh} $content;
	close $fh;
}
