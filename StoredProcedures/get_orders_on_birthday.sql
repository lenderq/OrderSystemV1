create or replace function get_orders_on_birthday()
returns table (client_id integer, total_amount numeric) as
$$
begin
    return query
	select Public."Clients".id, sum(Public."Orders".Сумма) from Public."Clients" 
	join Public."Orders" ON Public."Clients".id = Public."Orders"."ClientID"
	where Public."Orders".Статус = 'Выполнен' and 
	extract(month from Public."Clients".Дата_рождения) = 
	extract(month from Public."Orders".Дата_и_время) and
	extract(day from Public."Clients".Дата_рождения) = 
	extract(day from Public."Orders".Дата_и_время)
	group by Public."Clients".id;
end;
$$ language plpgsql;
