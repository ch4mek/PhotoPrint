import {
	Autoselect,
	AutoSelectWithChips,
} from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useQueryClient } from '@tanstack/react-query';
import isEqual from 'lodash/isEqual';
import { useMemo } from 'react';

import {
	GRANTING_PERMISSIONS_FORM_TITLE,
	REVOKING_ALL_PERMISSIONS_TITLE,
	GRANTING_PERMISSIONS_POPUP_TITLE,
	PERMISSION_REVOKING_POPUP_TITLE,
} from '../../constants';
import { revokingPermissionsPopupRenderer } from '../../renderers/revokingPermissionsPopupRenderer';
import { getPermissionAssignmentToastMessage } from '../../utils';
import { BILLABLE_WORKING_HOURS_PERMISSIONS_QUERY_KEY } from '../BillableWorkingHoursPermissionsGrid/hooks/useBillableWorkingHoursPermissions';
import { useCreatePermissionAssignment } from '../BillableWorkingHoursPermissionsGrid/hooks/useCreatePermissionAssignment';
import { usePermissionAssignmentsResources } from '../BillableWorkingHoursPermissionsGrid/hooks/usePermissionAssignmentsResources';

import { CANNOT_GRANT_PERMISSION_WITHOUT_USER_DEPARTMENT } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { Roles } from '@/constants/roles';
import { useToast } from '@/context/index';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { createToast } from '@/helpers/createToast';
import { extractKeyById } from '@/helpers/extractKey';
import { getFullName } from '@/helpers/utils';
import { useUsersLookup } from '@/queries/useUsersLookup';
import {
	PermissionTypeId,
	PermissionResourceTypeId,
} from '@/types/configuration';

import {
	ADD,
	initialBillableWorkingHoursPermissionsFormValues,
	REVOKE,
} from './constants';
import { useAllProjectsAssignedToUser } from './hooks/useAllProjectsAssignedToUser';
import { FormWrapper } from './styled';
import {
	ProjectOption,
	AddBillableWorkingHoursPermissionsFormProps,
} from './types';
import { getProjectKey } from './utils/getProjectKey';
import { validationSchema } from './validationSchema';

export function AddBillableWorkingHoursPermissionsForm({
	setIsOpenForm,
}: AddBillableWorkingHoursPermissionsFormProps) {
	const queryClient = useQueryClient();
	const toast = useToast();

	const handleClose = () => setIsOpenForm(false);

	const { data: users } = useUsersLookup({
		isActive: true,
		roles: [Roles.SE],
	});
	const userOptions = useMemo(
		() =>
			(users || []).map(({ id, firstName, lastName, departmentId }) => ({
				id,
				name: getFullName(firstName, lastName),
				departmentId,
			})),
		[users]
	);
	const {
		isPending: isLoading,
		mutate: mutateBillableWorkingHoursPermissions,
	} = useCreatePermissionAssignment({
		onSuccess(data, { userName, actionType }) {
			queryClient.invalidateQueries({
				queryKey: [BILLABLE_WORKING_HOURS_PERMISSIONS_QUERY_KEY],
			});

			toast.open(
				createToast(
					NotificationType.SUCCESS,
					<>
						{getPermissionAssignmentToastMessage(actionType)}
						<Typography variant="m-600">{userName}</Typography>
					</>
				)
			);
		},
	});

	return (
		<FormikWithBackup
			validationSchema={validationSchema}
			initialValues={initialBillableWorkingHoursPermissionsFormValues}
			validateOnBlur={false}
			onSubmit={() => {}}
			name={GRANTING_PERMISSIONS_FORM_TITLE}
			onClose={handleClose}
		>
			{({
				setFieldValue,
				values,
				errors,
				touched,
				setFieldTouched,
				validateField,
			}) => {
				const { userAvailableProjects, isLoadingUserAvailableProjects } =
					useAllProjectsAssignedToUser({
						userId: values?.user?.id,
						enabled: Boolean(values?.user?.id),
					});

				const {
					isLoading: isLoadingProjectResources,
					isFetching: isFetchingProjectResources,
					isError: isErrorProjectResources,
				} = usePermissionAssignmentsResources({
					userId: values?.user?.id,
					permissionType: PermissionTypeId.TrackBillableWorkingHours,
					resourceType: PermissionResourceTypeId.Project,
					queryOptions: {
						enabled: Boolean(values?.user?.id),
						onSuccess(data) {
							const userProjectOptions: ProjectOption[] = data.resources.map(
								({ resourceId, resourceName }) => ({
									id: Number(resourceId),
									name: resourceName,
								})
							);

							if (!isEqual(values?.projects, userProjectOptions)) {
								setFieldValue('projects', userProjectOptions);
							}
						},
					},
				});

				const handleOnChange = (field: string) => async (value: unknown) => {
					const setFields = async () => {
						await setFieldValue(field, value, true);
						setFieldTouched(field, true);
						validateField(field);
					};

					if (field === 'user') {
						await setFields();

						setFieldValue('projects', []);
					}

					const { user, projects } = values;

					const isUserAndProjectField = user && field === 'projects';

					if (isUserAndProjectField && !user.departmentId) {
						toast.open(
							createToast(
								NotificationType.ERROR,
								CANNOT_GRANT_PERMISSION_WITHOUT_USER_DEPARTMENT
							)
						);

						return;
					}

					if (isUserAndProjectField) {
						setFields();

						const isProjectWasAdded =
							(value as ProjectOption[]).length > projects.length;

						const resources = value
							? (value as ProjectOption[]).reduce(
									(acc, { id, name }) => ({ ...acc, [id]: name }),
									{}
							  )
							: {};

						mutateBillableWorkingHoursPermissions({
							permissionType: PermissionTypeId.TrackBillableWorkingHours,
							resourceType: PermissionResourceTypeId.Project,
							userId: user.id,
							resources,
							userName: user.name,
							actionType: isProjectWasAdded ? ADD : REVOKE,
						});
					}
				};

				const popupJSXContent = useMemo(
					() =>
						values.user
							? revokingPermissionsPopupRenderer(values.user.name)
							: undefined,
					[values.user]
				);

				const isAutoSelectWithChipsLoading =
					isLoadingUserAvailableProjects ||
					isLoadingProjectResources ||
					isFetchingProjectResources;

				const isAutoSelectWithChipsDisabled =
					userAvailableProjects.length === 0 || isErrorProjectResources;

				return (
					<PopUp
						title={GRANTING_PERMISSIONS_POPUP_TITLE}
						onClose={handleClose}
						type="form"
						showLoader={isLoading || userOptions === undefined}
					>
						<FormWrapper>
							{GRANTING_PERMISSIONS_FORM_TITLE}
							<Autoselect
								label="User"
								value={values?.user}
								valueField="name"
								options={userOptions ?? []}
								onChange={handleOnChange('user')}
								error={Boolean(errors.user && touched.user)}
								errorText={errors.user}
								getKey={extractKeyById}
								isRequired
							/>
							<AutoSelectWithChips
								label="Project"
								value={values.projects}
								valueField="name"
								withConfirmPopup
								popupTitle={PERMISSION_REVOKING_POPUP_TITLE}
								popupJSXContent={popupJSXContent}
								clearAllPopupTitle={PERMISSION_REVOKING_POPUP_TITLE}
								clearAllPopupContent={
									<Typography variant="m-400">
										{REVOKING_ALL_PERMISSIONS_TITLE}
										<Typography variant="m-600">
											{values?.user?.name}
										</Typography>
										?
									</Typography>
								}
								disabled={
									!isAutoSelectWithChipsLoading && isAutoSelectWithChipsDisabled
								}
								options={userAvailableProjects || []}
								onChange={handleOnChange('projects')}
								getKey={getProjectKey}
								error={Boolean(errors.projects && touched.projects)}
								errorText={errors.projects as any}
								groupByField="type"
								checkIfOptionEqualToValue={(
									option: { id: number },
									value: { id: number }
								) => option.id === value.id}
								chipsDisabled={false}
								isRequired
								disableClearable={!values.projects.length}
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
