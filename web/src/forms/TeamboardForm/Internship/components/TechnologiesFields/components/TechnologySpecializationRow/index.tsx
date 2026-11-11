import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { FieldArray, useFormikContext } from 'formik';
import { useMemo } from 'react';

import {
	ADD_HARD_SKILL_BUTTON_TITLE,
	createDefaultHardSkillItem,
	MAX_HARD_SKILLS_PER_SPECIALIZATION,
	REMOVE_SPECIALIZATION_ARIA_LABEL,
} from '../../constants';
import {
	AddMoreButtonWrapper,
	TechnologiesListWrapper,
	TechnologyFieldsRow,
	TechnologyInputsWrapper,
} from '../../styles';
import { TechnologiesFieldsValues } from '../../types';
import {
	getAvailableSpecializationOptions,
	getHardSkillsArrayPrefix,
	getSpecializationName,
	getSpecializationSeniorityName,
	getTechnologyError,
	getVisibleFieldError,
} from '../../utils';
import { TechnologyHardSkillRow } from '../TechnologyHardSkillRow';

import { AddMoreButton } from '@/components/AddMoreButton';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { AutoselectOption } from '@/types/common';

import {
	SPECIALIZATION_LABEL,
	SPECIALIZATION_SENIORITY_LABEL,
	SPECIALIZATION_SENIORITY_VALUE_FIELD,
	SPECIALIZATION_VALUE_FIELD,
} from './constants';
import type { TechnologySpecializationRowProps } from './types';

export function TechnologySpecializationRow({
	technologyIndex,
	technology,
	technologies,
	specializationOptions,
	seniorityOptions,
	getHardSkillOptions,
	onFieldChange,
	setFieldValue,
	onRemoveSpecialization,
	showRemoveSpecializationButton,
	technologyError,
	technologyTouched,
	hasAttemptedSubmit,
}: TechnologySpecializationRowProps) {
	const { setFieldError, setFieldTouched } =
		useFormikContext<TechnologiesFieldsValues>();
	const { specialization, seniority, hardSkills } = technology;
	const hasSpecialization = specialization != null;

	const specializationErrorText = getVisibleFieldError(
		technologyError?.specialization,
		technologyTouched?.specialization,
		hasAttemptedSubmit
	);
	const seniorityErrorText = getVisibleFieldError(
		technologyError?.seniority,
		technologyTouched?.seniority,
		hasAttemptedSubmit
	);

	const hardSkillsFieldPrefix = getHardSkillsArrayPrefix(technologyIndex);
	const specializationFieldName = getSpecializationName(technologyIndex);
	const specializationSeniorityFieldName =
		getSpecializationSeniorityName(technologyIndex);

	const resetHardSkillsValidationState = () => {
		setFieldError(hardSkillsFieldPrefix, undefined);
		setFieldTouched(hardSkillsFieldPrefix, false, false);
	};

	const resetSpecializationSeniorityValidationState = () => {
		setFieldError(specializationSeniorityFieldName, undefined);
		setFieldTouched(specializationSeniorityFieldName, false, false);
	};

	const handleSpecializationChange = (value: AutoselectOption | null) => {
		const nextHardSkills = (() => {
			if (!value) {
				return [];
			}

			if (hardSkills.length === 0) {
				return [createDefaultHardSkillItem()];
			}

			const nextHardSkillOptions = getHardSkillOptions(value.id);
			const filteredHardSkills = hardSkills.filter(({ hardSkill }) =>
				nextHardSkillOptions.some(({ id }) => id === hardSkill?.id)
			);

			return filteredHardSkills.length > 0
				? filteredHardSkills
				: [createDefaultHardSkillItem()];
		})();

		setFieldValue(hardSkillsFieldPrefix, nextHardSkills, false);

		if (!value) {
			setFieldValue(specializationSeniorityFieldName, null, false);
			resetSpecializationSeniorityValidationState();
		}

		setFieldValue(specializationFieldName, value, true);
		setFieldTouched(specializationFieldName, true, false);
		resetHardSkillsValidationState();
	};

	const availableSpecializations = useMemo(
		() =>
			getAvailableSpecializationOptions(
				specializationOptions,
				technologies,
				specialization?.id
			),
		[specialization?.id, specializationOptions, technologies]
	);

	const currentHardSkillOptions = useMemo(
		() => (specialization ? getHardSkillOptions(specialization.id) : []),
		[getHardSkillOptions, specialization]
	);

	const allowMoreHardSkills =
		hardSkills.length < MAX_HARD_SKILLS_PER_SPECIALIZATION;

	return (
		<TechnologyInputsWrapper>
			<TechnologyFieldsRow>
				<Autoselect<AutoselectOption>
					name={getSpecializationName(technologyIndex)}
					label={SPECIALIZATION_LABEL}
					value={specialization}
					options={availableSpecializations}
					onChange={handleSpecializationChange}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					valueField={SPECIALIZATION_VALUE_FIELD}
					error={Boolean(specializationErrorText)}
					errorText={specializationErrorText}
				/>
				<Autoselect
					name={getSpecializationSeniorityName(technologyIndex)}
					label={SPECIALIZATION_SENIORITY_LABEL}
					isRequired={hasSpecialization}
					value={seniority}
					options={seniorityOptions}
					onChange={onFieldChange(
						getSpecializationSeniorityName(technologyIndex)
					)}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					valueField={SPECIALIZATION_SENIORITY_VALUE_FIELD}
					error={Boolean(seniorityErrorText)}
					errorText={seniorityErrorText}
				/>
				{showRemoveSpecializationButton && (
					<ActionButton
						onClick={onRemoveSpecialization}
						aria-label={REMOVE_SPECIALIZATION_ARIA_LABEL}
					>
						<BasketIcon />
					</ActionButton>
				)}
			</TechnologyFieldsRow>

			{hasSpecialization && (
				<FieldArray name={hardSkillsFieldPrefix}>
					{({ push: pushSkill, remove: removeSkill }) => (
						<TechnologiesListWrapper isLastElement={!allowMoreHardSkills}>
							{hardSkills.map((skill, skillIndex) => (
								<TechnologyHardSkillRow
									key={skill.rowId}
									{...{
										technologyIndex,
										skillIndex,
										skill,
										technology,
										hasAttemptedSubmit,
									}}
									hardSkillOptions={currentHardSkillOptions}
									seniorityOptions={seniorityOptions}
									onFieldChange={onFieldChange}
									onRemove={() => removeSkill(skillIndex)}
									showRemoveButton={hardSkills.length > 1}
									isRemoveSpecializationButtonShown={
										showRemoveSpecializationButton
									}
									skillError={getTechnologyError(
										skillIndex,
										technologyError?.hardSkills
									)}
									skillTouched={technologyTouched?.hardSkills?.[skillIndex]}
								/>
							))}
							{allowMoreHardSkills && (
								<AddMoreButtonWrapper>
									<AddMoreButton
										handleClick={() => pushSkill(createDefaultHardSkillItem())}
										title={ADD_HARD_SKILL_BUTTON_TITLE}
									/>
								</AddMoreButtonWrapper>
							)}
						</TechnologiesListWrapper>
					)}
				</FieldArray>
			)}
		</TechnologyInputsWrapper>
	);
}
