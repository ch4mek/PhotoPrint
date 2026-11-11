export const findByName = <T extends { name: NameT }, NameT = string>(
	items: Array<T>,
	name: NameT
) => items.find((item) => item.name === name);
