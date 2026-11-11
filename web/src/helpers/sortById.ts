export const sortById = <T extends { id: number }>(items: T[]) =>
	[...items].sort((firstItem, secondItem) => firstItem.id - secondItem.id);
