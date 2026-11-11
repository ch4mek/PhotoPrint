import { HardSkillRow } from '../../../types';

import { ShortHardSkillInfo, UserHardSkillLookup } from '@/types/hardSkills';

export const mapHardSkillsLookupToRows = (
	items: Array<UserHardSkillLookup<ShortHardSkillInfo>>,
	onEditClick: (itemId?: string | number) => () => void
): Array<HardSkillRow> =>
	items.map(
		({ id, specializationTemplateName, isPrimary, seniorityLevelName }) => ({
			id,
			name: specializationTemplateName,
			seniority: seniorityLevelName,
			isPrimary,
			handleEditClick: onEditClick,
		})
	);
