import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { useMemo } from 'react';

import { REMOVE_HARD_SKILL_ARIA_LABEL } from '../../constants';
import { TechnologyFieldsRow } from '../../styles';
import {
	getAvailableHardSkillOptions,
	getHardSkillName,
	getHardSkillSeniorityName,
	getVisibleFieldError,
} from '../../utils';

import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';

import {
	HARD_SKILL_FIELD_LABEL,
	HARD_SKILL_SENIORITY_LABEL,
	HARD_SKILL_SENIORITY_VALUE_FIELD,
	HARD_SKILL_VALUE_FIELD,
} from './constants';
import type { TechnologyHardSkillRowProps } from './types';

export function TechnologyHardSkillRow({
	technologyIndex,
	skillIndex,
	skill,
	technology,
	hardSkillOptions,
	seniorityOptions,
	onFieldChange,
	onRemove,
	showRemoveButton,
	hasAttemptedSubmit,
	skillError,
	skillTouched,
	isRemoveSpecializationButtonShown,
}: TechnologyHardSkillRowProps) {
	const availableHardSkillOptions = useMemo(
		() =>
			getAvailableHardSkillOptions(
				hardSkillOptions,
				technology.hardSkills,
				skill.hardSkill?.id
			),
		[hardSkillOptions, skill.hardSkill?.id, technology.hardSkills]
	);

	const hardSkillErrorText = getVisibleFieldError(
		skillError?.hardSkill,
		skillTouched?.hardSkill,
		hasAttemptedSubmit
	);

	const shouldRenderButton =
		isRemoveSpecializationButtonShown || showRemoveButton;
	const isButtonDisabled =
		isRemoveSpecializationButtonShown && !showRemoveButton;

	return (
		<TechnologyFieldsRow>
			<Autoselect
				name={getHardSkillName(technologyIndex, skillIndex)}
				label={HARD_SKILL_FIELD_LABEL}
				isRequired
				value={skill.hardSkill}
				options={availableHardSkillOptions}
				onChange={onFieldChange(getHardSkillName(technologyIndex, skillIndex))}
				getKey={extractKeyById}
				checkIfOptionEqualToValue={checkIfOptionsEqualById}
				valueField={HARD_SKILL_VALUE_FIELD}
				error={Boolean(hardSkillErrorText)}
				errorText={hardSkillErrorText}
			/>
			<Autoselect
				name={getHardSkillSeniorityName(technologyIndex, skillIndex)}
				label={HARD_SKILL_SENIORITY_LABEL}
				value={skill.seniority}
				options={seniorityOptions}
				onChange={onFieldChange(
					getHardSkillSeniorityName(technologyIndex, skillIndex)
				)}
				getKey={extractKeyById}
				checkIfOptionEqualToValue={checkIfOptionsEqualById}
				valueField={HARD_SKILL_SENIORITY_VALUE_FIELD}
			/>
			{shouldRenderButton && (
				<ActionButton
					onClick={onRemove}
					disabled={isButtonDisabled}
					aria-label={REMOVE_HARD_SKILL_ARIA_LABEL}
				>
					{!isButtonDisabled && <BasketIcon />}
				</ActionButton>
			)}
		</TechnologyFieldsRow>
	);
}
