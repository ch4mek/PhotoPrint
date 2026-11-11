import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';

import { useConfiguration } from '@/context/Configuration';

import { ListWrapper, SkillSeniorityRow } from './styles';
import { UserHardSkillListProps } from './types';

export function UserHardSkillList({
	hardSkills,
	onSeniorityChange,
	errors = [],
	touched = [],
	senioritiesDisabled = false,
}: UserHardSkillListProps) {
	const {
		configurableValues: { generalSenioritySeniorityLevels },
	} = useConfiguration();

	const seniorityOptions = [...generalSenioritySeniorityLevels].reverse();

	return (
		<ListWrapper>
			{hardSkills.map(({ info, seniority }, index) => {
				const hardSkillSeniorityError = touched[index]
					? errors[index]?.seniority
					: undefined;

				return (
					<SkillSeniorityRow key={info.id}>
						<Autoselect
							disabled
							label="Hard skill"
							value={info}
							valueField="name"
							onChange={() => {}}
							options={[]}
							isRequired
						/>
						<Autoselect
							disabled={senioritiesDisabled}
							label="Seniority"
							value={seniority?.name ? seniority : null}
							valueField="name"
							onChange={onSeniorityChange(index)}
							options={seniorityOptions}
							error={Boolean(hardSkillSeniorityError)}
							errorText={hardSkillSeniorityError}
						/>
					</SkillSeniorityRow>
				);
			})}
		</ListWrapper>
	);
}
