export const sortByKey = <T>(items: T[], key: keyof T): T[] =>
	[...items].sort((a, b) => {
		const aValue = String(a[key]);
		const bValue = String(b[key]);

		return aValue.localeCompare(bValue);
	});

export const sortByName = <T extends { name: string }>(items: T[]) =>
	sortByKey(items, 'name');
