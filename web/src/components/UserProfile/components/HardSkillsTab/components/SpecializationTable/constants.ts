const SPECIALIZATION_NAME_COLUMN_WIDTH = 300;
const SPECIALIZATION_SENIORITY_COLUMN_WIDTH = 90;

export const EDIT_COLUMN_NO_DATA_OFFSET = 2;
export const EDIT_COLUMN_DEFAULT_OFFSET = 1;

export const SPECIALIZATION_HEADER_CELL_CLASS_NAME =
	'specialization-header-cell';

export const HARD_SKILLS_CELL_CLASS_NAME = 'specialization-hard-skills-cell';
export const FILLER_CELL_CLASS_NAME = 'filler-cell';

export const ODD_SPECIALIZATION_ROW_CLASS_NAME = 'odd-specialization-row';
export const EVEN_SPECIALIZATION_ROW_CLASS_NAME = 'even-specialization-row';
export const EDITABLE_COLUMN_CLASS_NAME = 'editable-column';

export const specializationColumnsWidth = {
	name: {
		width: SPECIALIZATION_NAME_COLUMN_WIDTH,
		fixed: true,
	},
	snapshot: {
		width: SPECIALIZATION_SENIORITY_COLUMN_WIDTH,
		fixed: true,
	},
	filler: 'minmax(0, 1fr)',
};

export const SPECIALIZATION_COLUMN_PROPS = {
	width: SPECIALIZATION_NAME_COLUMN_WIDTH,
	key: 'specialization',
	name: 'Specialization',
	sortable: true,
};

export const SPECIALIZATION_HARD_SKILL_COLUMN_PROPS = {
	width: SPECIALIZATION_NAME_COLUMN_WIDTH,
	key: 'hardSkill',
	name: 'Hard skill',
};
