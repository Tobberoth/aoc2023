use std::io::BufRead;

fn main() {
    let f = std::fs::File::open("input.txt").unwrap();
    let lines: Vec<_> = std::io::BufReader::new(f).lines().collect::<Result<_, _>>().unwrap();
    let seeds = lines[0].split(' ').filter_map(|c| c.parse::<u64>().ok() );
    let maps = generate_maps(&lines[2..]);

    let mut min = u64::MAX;
    for seed in seeds {
        let mut res = seed;
        for map in &maps {
            res = transform_seed(res, map);
        }
        min = std::cmp::min(min, res);
    }
    println!("{}", min);
}

fn generate_maps(lines: &[String]) -> Vec<Vec<(u64, u64, u64)>> {
    let mut maps: Vec<Vec<(u64, u64, u64)>> = Vec::new();
    for map in lines.split(|s| s == "") {
        let mut current_map: Vec<(u64, u64, u64)> = Vec::new();
        for map_range in &map[1..] {
            let values = map_range.split(' ').filter_map(|n| n.parse::<u64>().ok()).collect::<Vec<u64>>();
            let tup = (values[0], values[1], values[2]);
            current_map.push(tup);
        }
        current_map.sort_by_key(|t| t.1);
        maps.push(current_map);
    }
    maps
}

fn transform_seed(seed: u64, map: &Vec<(u64, u64, u64)>) -> u64 {
    for tup in map {
        if seed >= tup.1 && seed <= tup.1 + tup.2 {
            let diff = seed - tup.1;
            return tup.0 + diff;
        }
    }
    seed
}