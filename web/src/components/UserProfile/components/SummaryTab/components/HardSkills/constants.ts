export const hardSkillsColumns = {
	name: {
		key: 'name',
		sortable: false,
	},
	seniority: {
		key: 'seniority',
		sortable: false,
	},
};

export const columnsWidth: Record<keyof typeof hardSkillsColumns, number> = {
	name: 200,
	seniority: 93,
};

export const RESET_FILTER_COLUMN_INDEX =
	Object.values(hardSkillsColumns).length + 2;
