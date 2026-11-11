import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { spaces } from '@mms/mms-ui-library/theme';

import { Accordion } from '../Accordion';
import { Divider } from '../Divider';
import { UserHardSkillList } from '../UserHardSkillList';

import { useConfiguration } from '@/context/Configuration';

import {
	HardSkillAccordionWrapper,
	SeniorityAutoSelectWrapper,
	UserHardSkillListWrapper,
} from './styles';
import { SpecializationHardSkillListProps } from './types';

export function CollapsibleHardSkillList({
	onSpecializationSeniorityChange,
	onSeniorityChange,
	hardSkills,
	specialization,
	seniority,
	seniorityError,
	errors,
	touched,
}: SpecializationHardSkillListProps) {
	const {
		configurableValues: { seniorityLevels },
	} = useConfiguration();

	const seniorityOptions = [...seniorityLevels].reverse();

	const hasError =
		!!errors?.length &&
		touched?.some((isFieldTouched, index) => isFieldTouched && errors[index]);

	return (
		<Accordion label={specialization} hasError={hasError} spacingType="padding">
			<HardSkillAccordionWrapper>
				<SeniorityAutoSelectWrapper>
					<Autoselect
						label="Seniority"
						value={seniority?.name ?? null}
						valueField="name"
						onChange={onSeniorityChange}
						options={seniorityOptions.map((s) => s.name)}
						error={Boolean(seniorityError)}
						errorText={seniorityError}
						isRequired
					/>
				</SeniorityAutoSelectWrapper>

				{hardSkills.length > 0 && (
					<>
						<Divider margin={`${spaces.m}px 0`} />

						<UserHardSkillListWrapper>
							<UserHardSkillList
								hardSkills={hardSkills}
								onSeniorityChange={onSpecializationSeniorityChange}
								errors={errors}
								touched={touched}
							/>
						</UserHardSkillListWrapper>
					</>
				)}
			</HardSkillAccordionWrapper>
		</Accordion>
	);
}
