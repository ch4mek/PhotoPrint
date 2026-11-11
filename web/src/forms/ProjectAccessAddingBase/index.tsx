import { useMemo } from 'react';
import * as yup from 'yup';

import { FormikWithBackup } from '@/forms/components/FormikWithBackup';

import { ProjectAccessAddingFormContent } from './components/ProjectAccessAddingFormContent';
import { FORM_NAME } from './constants';
import {
	ProjectAssignFormDisableFlags,
	ProjectAssignFormProps,
	ProjectAssignFormValues,
} from './types';

export function ProjectAccessAddingForm({
	initialValues,
	handleClose,
	onSubmit,
	options,
	isLoading = false,
	label,
	validationSchema,
	projectSubValueField,
	isAssignEditable,
}: ProjectAssignFormProps) {
	const formInitialValue: ProjectAssignFormValues = useMemo(
		() => ({
			project: null,
			startDate: null,
			endDate: null,
			billability: true,
			fte: '',
			projectSeniority: null,
			user: null,

			...Object.entries(initialValues || {}).reduce(
				(acc, [key, value]) => ({
					...acc,
					[key]: value.value,
				}),
				{} as Partial<ProjectAssignFormValues>
			),
		}),
		[initialValues]
	);

	const formDisableFlags: ProjectAssignFormDisableFlags = useMemo(
		() => ({
			project: false,
			startDate: false,
			endDate: false,
			billability: false,
			fte: false,
			schedule: false,
			projectSeniority: false,
			user: false,

			...Object.entries(initialValues || {}).reduce(
				(flags, [key, value]) => ({
					...flags,
					[key]: Boolean(value?.disabled),
				}),
				{}
			),
		}),
		[initialValues]
	);

	const enabledFieldsValidationSchema = useMemo(
		() =>
			yup.object({
				...Object.entries(formInitialValue || {}).reduce(
					(schema, [key, value]) => ({
						...schema,
						...(!value?.disabled &&
							(isAssignEditable ||
								key === 'startDate' ||
								key === 'endDate') && {
								[key]: validationSchema.fields[key],
							}),
					}),
					{}
				),
			}),
		[formInitialValue, validationSchema]
	);

	return (
		<FormikWithBackup
			validateOnBlur={false}
			enableReinitialize
			initialValues={formInitialValue}
			onSubmit={onSubmit}
			onClose={handleClose}
			validationSchema={enabledFieldsValidationSchema}
			name={FORM_NAME}
			id={`${initialValues?.user?.value?.id}_${initialValues?.project?.value?.id}`}
		>
			{(formikProps) => (
				<ProjectAccessAddingFormContent
					{...{
						formikProps,
						formDisableFlags,
						options,
						isLoading,
						label,
						isAssignEditable,
						projectSubValueField,
					}}
					onClose={handleClose}
				/>
			)}
		</FormikWithBackup>
	);
}
