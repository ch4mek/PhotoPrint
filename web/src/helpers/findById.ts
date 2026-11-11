export const findById = <T extends { id: IdT }, IdT = string | number>(
	items: Array<T>,
	id: IdT
) => items.find((item) => item.id === id);
