import { SoftSkillRow, SoftSkillTableFilters } from '@/types/softSkills';

export const SOFT_SKILLS_COLUMNS_KEY = {
	softSkillId: 'id',
	softSkillName: 'softSkillName',
} as const;

export const SOFT_SKILL_SENIORITY_KEY = 'softSkillSeniority';

export const DEFAULT_SOFT_SKILLS_FILTER_VALUES: SoftSkillTableFilters = {
	softSkillName: '',
};

const SOFT_SKILL_COLUMN_NAME = 'Soft skill';

const SOFT_SKILL_NAME_COLUMN_WIDTH = 300;
const SOFT_SKILL_SNAPSHOT_COLUMN_WIDTH = 90;
const SOFT_SKILL_SNAPSHOT_EDITABLE_COLUMN_WIDTH = 110;

export const softSkillsColumnsWidths = {
	name: {
		width: SOFT_SKILL_NAME_COLUMN_WIDTH,
		fixed: true,
	},
	snapshot: (isColumnEditable: boolean) => ({
		width: isColumnEditable
			? SOFT_SKILL_SNAPSHOT_EDITABLE_COLUMN_WIDTH
			: SOFT_SKILL_SNAPSHOT_COLUMN_WIDTH,
		fixed: true,
	}),
	filler: {
		width: 'minmax(0, 1fr)',
	},
};

export const softSillsColumnProps = {
	key: SOFT_SKILLS_COLUMNS_KEY.softSkillName,
	name: SOFT_SKILL_COLUMN_NAME,
	sortable: true,
	width: SOFT_SKILL_NAME_COLUMN_WIDTH,
};

export const HEADER_BUTTON_TITLE = 'Add new entry';

export const SKELETON_SOFT_SKILL_ROW: SoftSkillRow = {
	id: 0,
	snapshots: {},
	softSkillName: '',
	showSkeleton: true,
};
export const SKELETON_SOFT_SKILL_ROW_AMOUNT = 1;
