import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { projectAccessEntityName } from '@/constants/formEntities';
import { ProjectAccessAddingForm } from '@/forms/ProjectAccessAddingBase';
import {
	projectAssignmentEditingSchema,
	projectAssignmentCreatingSchema,
} from '@/forms/schemas/projectAssignmentSchemas';

import { useGetProjectAccessFormValues } from './hooks/useGetProjectAccessFormValues';
import { ProjectAccessAddingFormProps } from './types';

export function ProjectAccessForm({
	handleClose,
	onSubmit,
	projectAssignId,
	projectData,
	showLoader = false,
}: ProjectAccessAddingFormProps) {
	const isEditMode = Boolean(projectAssignId);
	const { formInitialValue, formOptions, isFetching, isAssignEditable } =
		useGetProjectAccessFormValues(projectData, projectAssignId);

	const validationSchema = isEditMode
		? projectAssignmentEditingSchema
		: projectAssignmentCreatingSchema;

	return (
		<ProjectAccessAddingForm
			label={
				isEditMode
					? getEditingPopUpTitle(projectAccessEntityName)
					: getAddingPopUpTitle(projectAccessEntityName)
			}
			handleClose={handleClose}
			initialValues={formInitialValue}
			onSubmit={onSubmit}
			options={formOptions}
			isLoading={showLoader || isFetching}
			validationSchema={validationSchema}
			isAssignEditable={isEditMode ? Boolean(isAssignEditable) : true}
		/>
	);
}
