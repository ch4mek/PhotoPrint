import { useCallback, useMemo } from 'react';

import { FormikWithBackup } from '../components/FormikWithBackup';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';

import { ProjectExitFormContent } from './components/ProjectExitFormContent';
import {
	FORM_ENTITY_NAME,
	FORM_NAME,
	projectExitFormInitialValues,
} from './constants';
import { useSendProjectExitForm } from './hooks/useSendProjectExitForm';
import { schema } from './schema';
import {
	ProjectExitFormProps,
	ProjectExitFormValues,
	ValidatedProjectExitFormValues,
} from './types';
import { convertDataToSubmit, getInitialValues } from './utils';

export function ProjectExitForm({
	onClose,
	projectExit,
}: ProjectExitFormProps) {
	const { handleAddProjectExit, handleEditProjectExit, isLoading } =
		useSendProjectExitForm(onClose);

	const handleSubmit = useCallback(
		async (values: ProjectExitFormValues) => {
			const convertedValues = convertDataToSubmit(
				values as ValidatedProjectExitFormValues
			);

			if (projectExit) {
				await handleEditProjectExit({
					projectExitId: projectExit.id,
					...convertedValues,
				});
			} else {
				await handleAddProjectExit(convertedValues);
			}
		},
		[handleAddProjectExit, handleEditProjectExit, projectExit]
	);

	const initialValues = useMemo(
		() =>
			projectExit
				? getInitialValues(projectExit)
				: projectExitFormInitialValues,
		[projectExit]
	);

	const title = useMemo(
		() =>
			projectExit
				? getEditingPopUpTitle(FORM_ENTITY_NAME)
				: getAddingPopUpTitle(FORM_ENTITY_NAME),
		[projectExit]
	);

	return (
		<FormikWithBackup
			onSubmit={handleSubmit}
			validationSchema={schema}
			name={FORM_NAME}
			id={projectExit?.id}
			validateOnBlur={false}
			enableReinitialize
			{...{ onClose, initialValues }}
		>
			{({ handleOutsideClose }) => (
				<ProjectExitFormContent
					{...{ onClose, isLoading, handleOutsideClose, title }}
				/>
			)}
		</FormikWithBackup>
	);
}
