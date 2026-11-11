import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';

import { useConfiguration } from '@/context/Configuration';
import { softSkillFieldsData } from '@/forms/TeamboardForm/SeniorityForm/components/SkillsList/constants';
import {
	SkillSeniorityRow,
	SkillsListWrapper,
} from '@/forms/TeamboardForm/SeniorityForm/components/SkillsList/styles';
import { SkillsListProps } from '@/forms/TeamboardForm/SeniorityForm/components/SkillsList/types';

export function SkillsList({
	skills,
	onSeniorityChange,
	errors = [],
	touched = [],
}: SkillsListProps) {
	const {
		configurableValues: { generalSenioritySeniorityLevels },
	} = useConfiguration();

	const seniorityOptions = [...generalSenioritySeniorityLevels].reverse();

	const handleSkillChange = () => {};

	return (
		<SkillsListWrapper>
			{skills.map(({ softSkillId, softSkillName, seniority }, index) => {
				const skillSeniorityError = touched[index]
					? errors[index]?.seniority
					: undefined;

				return (
					<SkillSeniorityRow key={softSkillId}>
						<Autoselect
							value={softSkillName}
							onChange={handleSkillChange}
							options={[]}
							isRequired
							disabled
							{...softSkillFieldsData.softSkill}
						/>
						<Autoselect
							value={seniority}
							onChange={onSeniorityChange(index)}
							options={seniorityOptions}
							error={!!skillSeniorityError}
							errorText={skillSeniorityError}
							isRequired
							{...softSkillFieldsData.seniority}
						/>
					</SkillSeniorityRow>
				);
			})}
		</SkillsListWrapper>
	);
}
