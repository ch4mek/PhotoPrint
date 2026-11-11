import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useMemo } from 'react';

import { GROUP_FORM_POPUP_ENTITY_NAME } from '../constants';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { getFullName } from '@/helpers/formatName';
import { useGroupMutations } from '@/pages/AdminPanel/components/DepartmentsTab/hooks/useGroupMutations';
import { useGetDepartmentGroupManagers } from '@/queries/departments/useGetDepartmentGroupManagers';
import { useGetGroupById } from '@/queries/departments/useGetGroupById';
import {
	CreateGroupRequestBody,
	GroupMutationType,
	UpdateGroupRequestBody,
} from '@/types/department';
import { FormAutoselectOption } from '@/types/forms';

import {
	groupMutationFormFieldsLabels,
	groupMutationFormFieldsNames,
	FORM_NAME,
} from './constants';
import { validationSchema } from './schema';
import { GroupMutationFormProps, GroupMutationFormValues } from './types';

export function GroupMutationForm({
	id,
	departmentId,
	handleClose,
	refetch,
	mutationType,
}: GroupMutationFormProps) {
	const isEditMutationType = mutationType === GroupMutationType.Edit;
	const { data: group } = useGetGroupById(id, { enabled: isEditMutationType });

	const { handleMutation, isMutationLoading } = useGroupMutations(
		isEditMutationType ? GroupMutationType.Edit : GroupMutationType.Add,
		handleClose,
		refetch
	);

	const {
		data: departmentGroupManagers,
		isFetching,
		isLoading,
	} = useGetDepartmentGroupManagers(
		departmentId!,
		{
			hasActiveGroup: false,
		},
		{ enabled: Boolean(departmentId) }
	);
	const isDataLoading = isFetching || isLoading;

	const groupManagerOptions = useMemo<FormAutoselectOption[]>(() => {
		if (!departmentGroupManagers) {
			return [];
		}

		const availableManagers = departmentGroupManagers.map<FormAutoselectOption>(
			({ id: groupManagerId, firstName, lastName }) => ({
				id: groupManagerId,
				name: getFullName(firstName, lastName),
			})
		);

		if (!group) {
			return availableManagers;
		}

		const { id: currentId, managerFirstName, managerLastName } = group;

		if (!managerFirstName || !managerLastName || !currentId) {
			return availableManagers;
		}

		return [
			{ id: currentId, name: getFullName(managerFirstName, managerLastName) },
			...availableManagers,
		];
	}, [departmentGroupManagers, group]);

	const initialValues = useMemo<GroupMutationFormValues>(() => {
		if (!group) {
			return {
				name: '',
				groupManager: null,
			};
		}

		const { name, managerFirstName, managerLastName, groupManagerId } = group;

		const groupManager =
			!managerFirstName || !managerLastName
				? null
				: {
						id: groupManagerId,
						name: getFullName(managerFirstName, managerLastName),
				  };

		return {
			name,
			groupManager,
		};
	}, [id, group, groupManagerOptions]);

	const popupTitle = useMemo(() => {
		const popUpName = isEditMutationType
			? getEditingPopUpTitle(GROUP_FORM_POPUP_ENTITY_NAME)
			: getAddingPopUpTitle(GROUP_FORM_POPUP_ENTITY_NAME);

		return popUpName;
	}, [isEditMutationType]);

	const handleFormSubmit = ({
		name,
		groupManager,
	}: GroupMutationFormValues) => {
		if (isEditMutationType && id) {
			const mutationValues: UpdateGroupRequestBody = {
				id,
				name,
				groupManagerId: groupManager?.id,
				departmentId: departmentId || '',
			};

			handleMutation(mutationValues);
		} else {
			const mutationValues: CreateGroupRequestBody = {
				groupName: name,
				groupManagerId: groupManager?.id,
				departmentId: departmentId || '',
			};

			handleMutation(mutationValues);
		}
	};

	return (
		<FormikWithBackup
			enableReinitialize
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			{...{ initialValues, validationSchema, id }}
		>
			{({
				values: { groupManager },
				setFieldValue,
				handleSubmit,
				handleOutsideClose,
			}) => {
				const handleGroupManagerChange = (
					value: FormAutoselectOption<number> | null
				) => {
					setFieldValue(groupMutationFormFieldsNames.groupManager, value);
				};

				const showPopupLoader = isMutationLoading || isDataLoading;

				return (
					<PopUp
						title={popupTitle}
						type="form"
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						showLoader={showPopupLoader}
						controls={{
							positiveControl: {
								onClick: handleSubmit,
							},
							negativeControl: {
								onClick: handleClose,
							},
						}}
					>
						<FormWrapper>
							<TextField
								name={groupMutationFormFieldsNames.name}
								label={groupMutationFormFieldsLabels.name}
								size="fullWidth"
								onChange={setFieldValue}
								isRequired
							/>
							<Autoselect
								name={groupMutationFormFieldsNames.groupManager}
								label={groupMutationFormFieldsLabels.groupManager}
								value={groupManager}
								valueField="name"
								options={groupManagerOptions}
								onChange={handleGroupManagerChange}
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
