import { FieldArray, useFormikContext } from 'formik';
import { Fragment, useCallback } from 'react';

import { AddMoreButton } from '@/components/AddMoreButton';
import { Divider } from '@/components/Divider';

import { TechnologySpecializationRow } from './components/TechnologySpecializationRow';
import {
	ADD_SPECIALIZATION_BUTTON_TITLE,
	createDefaultTechnologyItem,
	MAX_SPECIALIZATIONS,
	TECHNOLOGIES_FIELD_NAME,
} from './constants';
import { useTechnologiesFieldsOptions } from './hooks/useTechnologiesFieldsOptions';
import { AddMoreButtonWrapper, TechnologiesListWrapper } from './styles';
import { TechnologiesFieldsValues } from './types';
import { getTechnologyError } from './utils';

export function TechnologiesFields() {
	const {
		values,
		errors,
		touched,
		submitCount,
		setFieldValue,
		setFieldTouched,
	} = useFormikContext<TechnologiesFieldsValues>();

	const technologies = values.technologies ?? [];
	const { specializationOptions, getHardSkillOptions, seniorityOptions } =
		useTechnologiesFieldsOptions(technologies);

	const allowMoreSpecializations = technologies.length < MAX_SPECIALIZATIONS;
	const hasAttemptedSubmit = submitCount > 0;

	const handleFieldChange = useCallback(
		(fieldName: string) => (value: unknown) => {
			setFieldValue(fieldName, value, true);
			setFieldTouched(fieldName, true);
		},
		[setFieldValue, setFieldTouched]
	);

	return (
		<FieldArray name={TECHNOLOGIES_FIELD_NAME}>
			{({ push: pushSpecialization, remove: removeSpecialization }) => {
				const handleRemoveSpecialization = (technologyIndex: number) => () => {
					removeSpecialization(technologyIndex);
				};

				const handleAddSpecialization = () => {
					pushSpecialization(createDefaultTechnologyItem());
				};

				return (
					<TechnologiesListWrapper isLastElement={!allowMoreSpecializations}>
						{technologies.map((technology, technologyIndex) => (
							<Fragment key={technology.rowId}>
								<TechnologySpecializationRow
									{...{
										technologyIndex,
										technology,
										technologies,
										specializationOptions,
										getHardSkillOptions,
										setFieldValue,
										hasAttemptedSubmit,
									}}
									seniorityOptions={seniorityOptions}
									onFieldChange={handleFieldChange}
									onRemoveSpecialization={handleRemoveSpecialization(
										technologyIndex
									)}
									showRemoveSpecializationButton={technologies.length > 1}
									technologyError={getTechnologyError(
										technologyIndex,
										errors.technologies
									)}
									technologyTouched={touched.technologies?.[technologyIndex]}
								/>
								{technologyIndex < technologies.length - 1 && <Divider />}
							</Fragment>
						))}
						{allowMoreSpecializations && (
							<AddMoreButtonWrapper>
								<AddMoreButton
									handleClick={handleAddSpecialization}
									title={ADD_SPECIALIZATION_BUTTON_TITLE}
								/>
							</AddMoreButtonWrapper>
						)}
					</TechnologiesListWrapper>
				);
			}}
		</FieldArray>
	);
}
