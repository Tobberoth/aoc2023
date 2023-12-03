use std::collections::{HashMap, HashSet};

fn main() {
    const DO_STEP_2: bool = false;
    let input = std::fs::read_to_string("input.txt").expect("Unable to read input");
    let lines: Vec<&str> = input.split('\n').collect();
    let mut number_objects_map = HashMap::new();
    let mut line_num = 0;
    for line in lines.as_slice() {
        let mut num: String = "".to_owned();
        let mut i = 0;
        for c in line.chars() {
            if c.is_digit(10) {
                num.push(c);
            } else if num != "" {
                number_objects_map.insert(
                    (line_num, i-1, i-num.len()),
                    num.parse::<i32>().unwrap());
                num = "".to_owned();
            }
            i += 1;
        }
        line_num += 1;
    }

    let mut line_num = 0;
    let mut sum: i64 = 0;
    for line in lines.as_slice() {
        let mut i = 0;
        for c in line.chars() {
            if !DO_STEP_2 {
                if !c.is_digit(10) && c != '.' && c != '\r' {
                    let res = get_adjacent_numbers(&number_objects_map, line_num, i);
                    sum += res.into_iter().sum::<i32>() as i64;
                }
            } else {
                if c == '*' {
                    let res = get_adjacent_numbers(&number_objects_map, line_num, i);
                    if res.len() == 2 {
                        sum += res.into_iter().product::<i32>() as i64;
                    }
                }

            }
            i += 1;
        }
        line_num += 1;
    }

    println!("{sum}");
}

fn get_adjacent_numbers(map: &HashMap<(i32, usize, usize), i32>, y: i32, x: usize) -> HashSet<i32> {
    let mut ret = HashSet::new();
    for kp in map {
        if kp.0.0 == y-1 && kp.0.2 <= x-1 && kp.0.1 >= x-1 {
            ret.insert(*kp.1);
        }
        if kp.0.0 == y-1 && kp.0.2 <= x+1 && kp.0.1 >= x+1 {
            ret.insert(*kp.1);
        }
        if kp.0.0 == y-1 && kp.0.2 <= x && kp.0.1 >= x {
            ret.insert(*kp.1);
        }
        if kp.0.0 == y+1 && kp.0.2 <= x-1 && kp.0.1 >= x-1 {
            ret.insert(*kp.1);
        }
        if kp.0.0 == y+1 && kp.0.2 <= x+1 && kp.0.1 >= x+1 {
            ret.insert(*kp.1);
        }
        if kp.0.0 == y+1 && kp.0.2 <= x && kp.0.1 >= x {
            ret.insert(*kp.1);
        }
        if kp.0.0 == y && kp.0.2 <= x+1 && kp.0.1 >= x+1 {
            ret.insert(*kp.1);
        }
        if kp.0.0 == y && kp.0.2 <= x-1 && kp.0.1 >= x-1 {
            ret.insert(*kp.1);
        }
    }
    return ret;
}