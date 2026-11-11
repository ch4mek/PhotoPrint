import { ShortHardSkillInfo, UserHardSkillLookup } from '@/types/hardSkills';

import { HardSkillsSectionRow } from './types';

export const convertHardSkillsLookupToRows = (
	items: Array<UserHardSkillLookup<ShortHardSkillInfo>>
): HardSkillsSectionRow[] =>
	items.map(
		({ id, specializationTemplateName, isPrimary, seniorityLevelName }) => ({
			id,
			name: specializationTemplateName,
			seniority: seniorityLevelName,
			isPrimary,
		})
	);

export const getRowKey = (row: HardSkillsSectionRow) => row.id;
