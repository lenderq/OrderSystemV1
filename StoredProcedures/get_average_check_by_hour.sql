create or replace function get_average_check_by_hour()
returns table (Час integer, Средний_чек numeric) as
$$
begin
    return query
    select extract(hour from Public."Orders".Дата_и_время) as Час,
    sum(Public."Orders".Сумма)/count(Public."Orders".id) as Средний_чек 
    from Public."Orders" where Public."Orders".Статус = 'Выполнен'
    group by extract(hour from Public."Orders".Дата_и_время)
    order by extract(hour from Public."Orders".Дата_и_время) desc;
end;
$$ language plpgsql;