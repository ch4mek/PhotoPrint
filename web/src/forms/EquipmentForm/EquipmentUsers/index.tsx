import { useQueryClient } from '@tanstack/react-query';
import { FormikHelpers } from 'formik';
import { memo, useEffect, useLayoutEffect, useMemo, useState } from 'react';

import { HistoryItem } from '../types';
import { checkBrokenStatus } from '../utils/checkBrokenStatus';

import { FIELD_IS_REQUIRED } from '@/constants/errors';
import {
	EQUIPMENT_FUTURE_RETURN_DATE_ERROR,
	ISSUE_DATE_SHOULD_NOT_BE_LESS_LAST_RETURN_DATE,
	NotificationType,
} from '@/constants/notifications';
import {
	ASSIGNMENTS_HISTORY_QUERY_KEY,
	EquipmentQueriesKeys,
} from '@/constants/queriesKeys';
import { SortDirection } from '@/constants/sort';
import { useEquipmentContext } from '@/context/EquipmentContext/Equipment';
import { useToast } from '@/context/Toast';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { isDateMoreThanCurrentDate } from '@/forms/EquipmentForm/EquipmentUsers/UserForm/utils/isDateMoreThanCurrentDate';
import { createToast } from '@/helpers/createToast';
import dayjs from '@/lib/dayjs';
import { useDeleteActiveHolder } from '@/queries/equipment/useDeleteActiveHolder';
import { useGetEquipmentHistory } from '@/queries/equipment/useGetEquipmentHistory';
import { useUpdateActiveHolder } from '@/queries/equipment/useUpdateActiveHolder';
import type { ActiveHolder, EquipmentHistoryItem } from '@/types/equipment';

import { EquipmentUsersProps } from './types';
import { UserEquipmentForm } from './UserForm';
import {
	FORM_NAME,
	initUserFilter,
	PAGE,
	ROWS_PER_PAGE,
	userFormFields,
} from './UserForm/constants';
import { validationSchema } from './UserForm/schema';
import type { InitUserFilterState } from './UserForm/types';

function EquipmentUsersComponent({
	setHistoryItems,
	historyItems,
	equipmentId,
	onLoad,
	onClose,
	status,
}: EquipmentUsersProps) {
	const [initialValues, setInitialValues] =
		useState<InitUserFilterState>(initUserFilter);
	const queryClient = useQueryClient();
	const toast = useToast();

	const { data, isLoading } = useGetEquipmentHistory({
		toast,
		id: equipmentId,
		page: PAGE,
		rowsPerPage: ROWS_PER_PAGE,
		isDeletedFilter: false,
		orderBy: SortDirection.DESC,
	});

	const [lastUser, setLastUser] = useState<EquipmentHistoryItem | null>();

	const { equipmentModel, setEquipmentModel } = useEquipmentContext();

	const { mutate: updateActiveHolder } = useUpdateActiveHolder(toast);
	const { mutate: deleteActiveHolder } = useDeleteActiveHolder(toast);

	const isBrokenStatus = useMemo(() => checkBrokenStatus(status), [status]);

	const isEquipmentAssigned =
		Boolean(lastUser?.issueDate) && Number.isInteger(lastUser?.id);
	const isEquipmentReturned = Boolean(lastUser?.returnDate);

	useEffect(() => {
		if (!isLoading) {
			setHistoryItems((data?.items as HistoryItem[]) || []);
			setLastUser(data ? data.items[0] : null);
		}
	}, [data, isLoading]);

	useLayoutEffect(() => {
		onLoad(isLoading);
	}, [isLoading]);

	useEffect(() => {
		if (
			!historyItems ||
			historyItems.length === 0 ||
			historyItems[0]?.returnDate !== null
		) {
			setInitialValues(initUserFilter);
			setEquipmentModel({ ...equipmentModel, activeHolder: null });
		} else {
			setLastUser(historyItems[0]);
		}
	}, [historyItems]);

	useEffect(() => {
		if (lastUser && isEquipmentAssigned && !isEquipmentReturned) {
			setInitialValues({
				returnDate: lastUser.returnDate?.isValid() ? lastUser.returnDate : null,
				approver: {
					id: lastUser?.approverId,
					fullName: lastUser?.approverFullName,
				},
				selectedUser: lastUser,
				issueDate: dayjs(lastUser?.issueDate),
				id: Number(equipmentId),
			});
			setEquipmentModel((prev) => ({
				...prev,
				activeHolder: {
					id: lastUser?.id,
					issueDate: lastUser?.issueDate,
				} as ActiveHolder,
			}));
		}
	}, [lastUser, data, isEquipmentAssigned, isEquipmentReturned, equipmentId]);

	const handleSubmitForm = (
		values: InitUserFilterState,
		formik: FormikHelpers<InitUserFilterState>
	) => {
		const handleSuccessResponse = (response: EquipmentHistoryItem) => {
			queryClient.refetchQueries({
				queryKey: [EquipmentQueriesKeys.history],
			});

			setHistoryItems((prev) => [response, ...prev] as HistoryItem[]);
			setLastUser(response);

			queryClient.invalidateQueries({
				queryKey: [EquipmentQueriesKeys.equipments],
			});
			queryClient.invalidateQueries({
				queryKey: [ASSIGNMENTS_HISTORY_QUERY_KEY],
			});
		};
		const { selectedUser, returnDate, issueDate, approver } = values;

		const isIssueDateSameOrAfterLastReturnData = dayjs(issueDate).isSameOrAfter(
			lastUser?.returnDate,
			'day'
		);

		const shouldRequireReturnDate =
			!returnDate && !isEquipmentReturned && isEquipmentAssigned;

		if (shouldRequireReturnDate) {
			formik.setFieldError(userFormFields.returnDate.name, FIELD_IS_REQUIRED);
		}

		if (!selectedUser || !approver || !issueDate || shouldRequireReturnDate) {
			return;
		}

		const isReturnDateMoreThanCurrentDate =
			returnDate?.isValid() && isDateMoreThanCurrentDate(returnDate.toDate());

		if (isReturnDateMoreThanCurrentDate) {
			toast.open(
				createToast(NotificationType.ERROR, EQUIPMENT_FUTURE_RETURN_DATE_ERROR)
			);

			return;
		}

		if (isEquipmentAssigned && !isEquipmentReturned) {
			deleteActiveHolder(
				{
					equipmentId,
					returnDate: returnDate!,
				},
				{
					onSuccess: handleSuccessResponse,
				}
			);

			return;
		}

		if (equipmentId) {
			if (lastUser?.returnDate && !isIssueDateSameOrAfterLastReturnData) {
				toast.open(
					createToast(
						NotificationType.ERROR,
						ISSUE_DATE_SHOULD_NOT_BE_LESS_LAST_RETURN_DATE
					)
				);

				return;
			}

			updateActiveHolder(
				{
					userId: Number(selectedUser?.id),
					approverId: Number(approver?.id),
					issueDate,
					equipmentId,
				},
				{
					onSuccess: handleSuccessResponse,
				}
			);
		}
	};

	return (
		<FormikWithBackup
			onSubmit={handleSubmitForm}
			enableReinitialize
			validateOnChange
			validateOnBlur={false}
			name={FORM_NAME}
			id={equipmentId}
			{...{ initialValues, validationSchema, onClose }}
		>
			{(formik) => (
				<UserEquipmentForm
					{...{
						formik,
						isBrokenStatus,
						isEquipmentAssigned,
						isEquipmentReturned,
					}}
					equipmentHistoryProps={{
						equipmentId,
						setHistoryItems,
						status,
					}}
				/>
			)}
		</FormikWithBackup>
	);
}

export const EquipmentUsers = memo(EquipmentUsersComponent);
