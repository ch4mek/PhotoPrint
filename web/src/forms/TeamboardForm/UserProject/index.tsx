import { useMemo } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { projectAccessEntityName } from '@/constants/formEntities';
import { useConfiguration } from '@/context/Configuration';
import { ProjectAccessAddingForm } from '@/forms/ProjectAccessAddingBase';
import {
	ProjectAssignFormOptions,
	ProjectAssignFormInitialValues,
} from '@/forms/ProjectAccessAddingBase/types';
import {
	userProjectAssignmentEditingSchema,
	projectAssignmentCreatingSchema,
} from '@/forms/schemas/projectAssignmentSchemas';
import { getFullName } from '@/helpers/formatName';
import { useUserProjectAssignmentById } from '@/queries/teamboard/useUserProjectAssignmentById';
import { useProjectsLookup } from '@/queries/useProjectsLookup';
import { useUsersLookup } from '@/queries/useUsersLookup';

import { PROJECT_SUB_VALUE_FIELD } from './constants';
import { TeamboardUserProjectsFormProps } from './types';
import { convertProjectAssignToInitialFormValues } from './utils';

export function TeamboardUserProjectsForm({
	handleClose,
	onSubmit,
	projectAssignId,
	showLoader = false,
	user,
}: TeamboardUserProjectsFormProps) {
	const isEditMode = Boolean(projectAssignId);
	const { data: usersLookup, isFetching: areUsersFetching } = useUsersLookup({
		isActive: true,
		queryOptions: {
			enabled: !isEditMode,
		},
	});
	const { data: projectsLookup, isFetching: areProjectsFetching } =
		useProjectsLookup({
			isActive: true,
			isPublic: false,
		});

	const {
		configurableValues: { seniorityLevels, countries },
	} = useConfiguration();

	const { data, isFetching: isAssignmentFetching } =
		useUserProjectAssignmentById(projectAssignId);

	const userOptions = useMemo(
		() =>
			(usersLookup || []).map(({ id, lastName, firstName }) => ({
				id,
				fullName: getFullName(firstName, lastName),
			})),
		[usersLookup]
	);

	const formOptions: ProjectAssignFormOptions = useMemo(
		() => ({
			users: userOptions,
			projects: projectsLookup || [],
		}),
		[userOptions, projectsLookup]
	);

	const formInitialValue: ProjectAssignFormInitialValues = useMemo(() => {
		if (!projectAssignId) {
			return {
				user: {
					value: user,
					disabled: true,
				},
			};
		}

		if (!data) {
			return {};
		}

		return convertProjectAssignToInitialFormValues(
			data,
			user,
			projectsLookup || [],
			seniorityLevels,
			countries,
			true
		);
	}, [data, user, projectAssignId, projectsLookup, seniorityLevels, countries]);

	const showPopupLoader =
		showLoader ||
		areProjectsFetching ||
		areUsersFetching ||
		isAssignmentFetching;

	const validationSchema = isEditMode
		? userProjectAssignmentEditingSchema
		: projectAssignmentCreatingSchema;

	return (
		<ProjectAccessAddingForm
			label={
				projectAssignId
					? getEditingPopUpTitle(projectAccessEntityName)
					: getAddingPopUpTitle(projectAccessEntityName)
			}
			handleClose={handleClose}
			initialValues={formInitialValue}
			onSubmit={onSubmit}
			options={formOptions}
			isLoading={showPopupLoader}
			isAssignEditable={
				projectAssignId ? Boolean(data?.isAssignEditable) : true
			}
			validationSchema={validationSchema}
			projectSubValueField={PROJECT_SUB_VALUE_FIELD}
		/>
	);
}
