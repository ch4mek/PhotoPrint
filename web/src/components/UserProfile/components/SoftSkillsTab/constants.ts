import { SoftSkillTableFilters } from '@/types/softSkills';

export const SOFT_SKILLS_COLUMNS_KEY = {
	softSkillId: 'id',
	softSkillName: 'softSkillName',
} as const;

export const SOFT_SKILL_SENIORITY_KEY = 'softSkillSeniority';

export const DEFAULT_SOFT_SKILLS_FILTER_VALUES: SoftSkillTableFilters = {
	softSkillName: '',
};

const SOFT_SKILL_NAME_COLUMN_WIDTH = 300;
const SOFT_SKILL_SNAPSHOT_COLUMN_WIDTH = 90;

export const softSkillsColumnsWidths = {
	name: {
		width: SOFT_SKILL_NAME_COLUMN_WIDTH,
		fixed: true,
	},
	snapshot: {
		width: SOFT_SKILL_SNAPSHOT_COLUMN_WIDTH,
		fixed: true,
	},
	filler: {
		width: 'minmax(0, 1fr)',
	},
};

export const softSillsColumnProps = {
	key: SOFT_SKILLS_COLUMNS_KEY.softSkillName,
	sortable: true,
	width: SOFT_SKILL_NAME_COLUMN_WIDTH,
};

export const FILLER_CELL_CLASS_NAME = 'filler-cell';

export const EDIT_COLUMN_NO_DATA_OFFSET = 2;
export const EDIT_COLUMN_DEFAULT_OFFSET = 1;
