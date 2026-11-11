import { Typography } from '@mms/mms-ui-library/components/Typography';

import { formatHardSkillsText, formatNameWithSeniority } from '../../utils';

import { TechnologyRowWrapper } from './styles';
import { TechnologyRowProps } from './types';

export function TechnologyRow({ technology }: TechnologyRowProps) {
	const { specializationTemplateName, seniorityLevelName, hardSkills } =
		technology;

	const specializationTitle = formatNameWithSeniority(
		specializationTemplateName,
		seniorityLevelName
	);
	const hardSkillsText = formatHardSkillsText(hardSkills, seniorityLevelName);

	return (
		<TechnologyRowWrapper>
			<Typography variant="s-700">{specializationTitle}</Typography>
			<Typography variant="m-400">{hardSkillsText}</Typography>
		</TechnologyRowWrapper>
	);
}
