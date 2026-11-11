import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useQueryClient } from '@tanstack/react-query';
import { FormikHelpers } from 'formik';
import { useCallback, useContext, useMemo } from 'react';

import { GroupInfoContext } from '../AdminPanelForms/UserForm/GroupMembersTab/context/GroupInfoContext';

import { NotificationType } from '@/constants/notifications';
import { GroupQueriesKeys, UsersQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { createToast } from '@/helpers/createToast';
import { extractKeyById } from '@/helpers/extractKey';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useAddUserToGroup } from '@/queries/groups/useAddUserToGroup';
import { FormUserAutoselectOption } from '@/types/forms';

import {
	ADD_GROUP_MEMBER_FORM_LABEL,
	FIELD_LABELS,
	initialAddGroupMemberFormValues,
	USER_ADDED_TOAST_CONTENT,
} from './constants';
import { addGroupMemberValidationSchema } from './schema';
import {
	AddGroupMemberFormFields,
	AddGroupMemberFormProps,
	AddGroupMemberFormValues,
} from './types';

export function AddGroupMemberForm({
	onClose,
	options,
}: AddGroupMemberFormProps) {
	const { groupId } = useContext(GroupInfoContext);
	const queryClient = useQueryClient();

	const toast = useToast();

	const { mutateAsync } = useAddUserToGroup({
		groupId,
		options: {
			onError: createOnErrorHandler(toast),
			onSuccess: () => {
				queryClient.invalidateQueries({
					queryKey: [GroupQueriesKeys.groupMembers],
				});
				queryClient.invalidateQueries({
					queryKey: [GroupQueriesKeys.departmentUsersWithoutGroup],
				});
				queryClient.invalidateQueries({ queryKey: [UsersQueriesKeys.users] });
				onClose();
			},
		},
	});

	const handleSubmit = useCallback(
		async (
			{ user }: AddGroupMemberFormValues,
			{ setSubmitting }: FormikHelpers<AddGroupMemberFormValues>
		) => {
			try {
				if (user) {
					await mutateAsync(user.id);
					toast.open(
						createToast(
							NotificationType.SUCCESS,
							<Typography variant="m-400">
								<Typography variant="m-600">{user.fullName}</Typography>{' '}
								{USER_ADDED_TOAST_CONTENT}
							</Typography>
						)
					);
				}
			} finally {
				setSubmitting(false);
			}
		},
		[groupId]
	);

	const autoselectOptions: Array<FormUserAutoselectOption> = useMemo(
		() =>
			options[AddGroupMemberFormFields.USER].map(({ id, fullName }) => ({
				id,
				fullName,
			})),
		[options]
	);

	return (
		<FormikWithBackup
			onSubmit={handleSubmit}
			onClose={onClose}
			initialValues={initialAddGroupMemberFormValues}
			validateOnBlur={false}
			validationSchema={addGroupMemberValidationSchema}
			enableReinitialize
			name={ADD_GROUP_MEMBER_FORM_LABEL}
			id={groupId}
		>
			{({
				values,
				setFieldValue,
				errors,
				touched,
				submitForm,
				handleOutsideClose,
			}) => {
				const { user } = values;

				const handleFieldChange =
					(field: AddGroupMemberFormFields) => (value: unknown) => {
						setFieldValue(field, value);
					};

				return (
					<PopUp
						onClose={onClose}
						onShadowClick={handleOutsideClose}
						title={ADD_GROUP_MEMBER_FORM_LABEL}
						type="form"
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: onClose },
						}}
					>
						<Autoselect
							value={user}
							valueField="fullName"
							getKey={extractKeyById}
							onChange={handleFieldChange(AddGroupMemberFormFields.USER)}
							label={FIELD_LABELS[AddGroupMemberFormFields.USER]}
							options={autoselectOptions}
							error={
								Boolean(errors[AddGroupMemberFormFields.USER]) &&
								Boolean(touched[AddGroupMemberFormFields.USER])
							}
							errorText={errors[AddGroupMemberFormFields.USER]}
							checkIfOptionEqualToValue={checkIfOptionsEqualById}
							isRequired
						/>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
