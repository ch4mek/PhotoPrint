import { useCallback, useContext, useMemo, useState } from 'react';

import { PersonalDataContext } from '../context/personalDataContext';
import {
	DevelopmentPlanMutationMode,
	UseDevelopmentPlanFormStateProps,
} from '../types';
import { mapDevelopmentPlanToRow } from '../utils';
import { getNumericIdForAddedItem } from '../utils/getNumericIdForAddedItem';

import { DATE_FORMAT, YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { useToast } from '@/context/Toast';
import {
	DevelopmentPlanPopUp,
	StatusOfItem,
} from '@/forms/TeamboardForm/Conversation/types';
import { DevelopmentPlanFormValues } from '@/forms/TeamboardForm/DevelopmentPlan/types';
import { convertDevelopmentPlanToFormInitialState } from '@/forms/TeamboardForm/DevelopmentPlan/utils';
import { DevelopmentPlanApprovingFormValues } from '@/forms/TeamboardForm/DevelopmentPlanApproving/types';
import { getEnumKeyByValue } from '@/helpers/getEnumKeyByValue';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { formatDevelopmentPlanDates } from '@/pages/TeamBoardUser/utils/formatDevelopmentPlanDates';
import { sortDevelopmentPlanRowsDateByDesc } from '@/pages/TeamBoardUser/utils/sortDevelopmentPlanRowsDateByDesc';
import { useCreateDevelopmentPlan } from '@/queries/teamboard/useCreateDevelopmentPlan';
import { useDeleteDevelopmentPlan } from '@/queries/teamboard/useDeleteDevelopmentPlan';
import { useUpdateDevelopmentPlan } from '@/queries/teamboard/useUpdateDevelopmentPlan';
import { useUpdateDevelopmentPlanStatus } from '@/queries/useUpdateDevelopmentPlanStatus';
import {
	DevelopmentPlanRow,
	DevelopmentPlanStatus,
} from '@/types/developmentPlan';
import { TeamboardTableMode } from '@/types/teamboard';

export const useDevelopmentPlanMutations = ({
	refetch,
	setRows,
	rows,
	developmentPlans,
	setDevelopmentPlans,
	mode,
	conversationId,
}: UseDevelopmentPlanFormStateProps) => {
	const [mutationMode, setMutationMode] = useState(
		DevelopmentPlanMutationMode.None
	);
	const { handleOpen, isOpen, handleClose, id } = useShowModal<number>();
	const [
		selectedDevelopmentPlanStartDate,
		setSelectedDevelopmentPlanStartDate,
	] = useState<string>();
	const { userId } = useContext(PersonalDataContext);
	const toast = useToast();

	const handleSuccessMutation = (enableRefetch = true) => {
		if (enableRefetch) {
			refetch();
		}

		handleClose();
	};

	const { mutateAsync: createDevelopmentPlanMutation } =
		useCreateDevelopmentPlan(toast);
	const { mutateAsync: updateDevelopmentPlanMutation } =
		useUpdateDevelopmentPlan(toast);
	const { mutateAsync: updateDevelopmentPlanStatus } =
		useUpdateDevelopmentPlanStatus(toast);
	const {
		mutateAsync: deleteDevelopmentPlanMutation,
		isPending: isDeleteLoading,
	} = useDeleteDevelopmentPlan(toast);

	const handleAddClick = useCallback(() => {
		setMutationMode(DevelopmentPlanMutationMode.AddOrEdit);
		handleOpen();
	}, []);

	const handleEditOptionClick = useCallback(
		(developmentPlanId?: number | string) => () => {
			setMutationMode(DevelopmentPlanMutationMode.AddOrEdit);
			handleOpen(Number(developmentPlanId));
		},
		[]
	);

	const handleDeleteOptionClick = useCallback(
		(developmentPlanId: number, startDate: string) => () => {
			setMutationMode(DevelopmentPlanMutationMode.Delete);
			setSelectedDevelopmentPlanStartDate(startDate);
			handleOpen(developmentPlanId);
		},
		[]
	);

	const handleApproveOptionClick = useCallback(
		(developmentPlanId: number, startDate: string) => () => {
			setMutationMode(DevelopmentPlanMutationMode.Approve);
			setSelectedDevelopmentPlanStartDate(startDate);
			handleOpen(developmentPlanId);
		},
		[]
	);

	const handleRejectOptionClick = useCallback(
		(developmentPlanId: number) => async () => {
			await updateDevelopmentPlanStatus({
				developmentPlanId,
				developmentPlanStatus: getEnumKeyByValue(
					DevelopmentPlanStatus,
					DevelopmentPlanStatus.Rejected
				),
			});

			refetch();
		},
		[]
	);

	const handleCloseClick = useCallback(() => {
		handleClose();
		setMutationMode(DevelopmentPlanMutationMode.None);
		setSelectedDevelopmentPlanStartDate(undefined);
	}, []);

	const createDevelopmentPlanQuery = async (
		values: DevelopmentPlanFormValues
	) => {
		const { startDate, deadlineDate, notes, summary } = values;
		await createDevelopmentPlanMutation({
			...formatDevelopmentPlanDates(startDate, deadlineDate),
			summary,
			notes: notes || null,
			userId: Number(userId),
		});

		handleSuccessMutation();
	};

	const updateDevelopmentPlanQuery = async (
		values: DevelopmentPlanFormValues
	) => {
		const { startDate, deadlineDate, notes, summary } = values;
		await updateDevelopmentPlanMutation({
			id: id!,
			...formatDevelopmentPlanDates(startDate, deadlineDate),
			summary,
			notes: notes || null,
		});

		handleSuccessMutation();
	};

	const createDevelopmentPlan = (values: DevelopmentPlanFormValues) => {
		if (rows && setRows && setDevelopmentPlans && developmentPlans) {
			const { startDate, deadlineDate, ...restValues } = values;
			const dataToSave: DevelopmentPlanPopUp = {
				...formatDevelopmentPlanDates(startDate, deadlineDate),
				...restValues,
				id: getNumericIdForAddedItem(rows),
				userId: Number(userId),
				status: StatusOfItem.ADDED,
				conversationId,
			};

			setRows(
				sortDevelopmentPlanRowsDateByDesc([
					mapDevelopmentPlanToRow(dataToSave, handleEditOptionClick),
					...rows,
				])
			);
			setDevelopmentPlans([
				{ ...dataToSave, status: StatusOfItem.ADDED },
				...developmentPlans,
			]);

			handleSuccessMutation(false);
		}
	};

	const updateDevelopmentPlan = (values: DevelopmentPlanFormValues) => {
		if (rows && setRows && developmentPlans && setDevelopmentPlans) {
			const { startDate, deadlineDate, ...restValues } = values;
			const dataToSave: DevelopmentPlanPopUp = {
				...formatDevelopmentPlanDates(startDate, deadlineDate),
				...restValues,
				id: id!,
				userId: Number(userId),
				status: StatusOfItem.EDITED,
				conversationId,
			};

			setRows(
				rows.map((item) => {
					if (item.id === id) {
						return mapDevelopmentPlanToRow(dataToSave, handleEditOptionClick);
					}

					return item;
				})
			);

			if (developmentPlans.every((devPlan) => devPlan.id !== id)) {
				setDevelopmentPlans([dataToSave, ...developmentPlans]);
			} else {
				setDevelopmentPlans(
					developmentPlans.map((devPlan) => {
						if (devPlan.id === id) {
							return { ...dataToSave, status: StatusOfItem.ADDED };
						}

						return devPlan;
					})
				);
			}

			handleSuccessMutation(false);
		}
	};

	const getDevelopmentPlanById = useMemo(
		() =>
			(idOfDevPlan: number): DevelopmentPlanFormValues => {
				const { endDate, deadlineDate, startDate, ...rest } = rows?.find(
					(row) => row.id === idOfDevPlan
				) as DevelopmentPlanRow;

				return convertDevelopmentPlanToFormInitialState({
					endDate: endDate
						? dayjs(endDate, DATE_FORMAT).format(YYYY_MM_DD_DAYJS_DATE_FORMAT)
						: null,
					startDate: dayjs(startDate, DATE_FORMAT).format(
						YYYY_MM_DD_DAYJS_DATE_FORMAT
					),
					deadlineDate: deadlineDate
						? dayjs(deadlineDate, DATE_FORMAT).format(
								YYYY_MM_DD_DAYJS_DATE_FORMAT
						  )
						: null,
					...rest,
				});
			},
		[rows]
	);

	const handleAddOrEditFormSubmit = (values: DevelopmentPlanFormValues) => {
		const updateDevelopmentFn =
			mode === TeamboardTableMode.PopUp
				? updateDevelopmentPlan
				: updateDevelopmentPlanQuery;
		const createDevelopmentPlanFn =
			mode === TeamboardTableMode.PopUp
				? createDevelopmentPlan
				: createDevelopmentPlanQuery;

		const submitFunction = id ? updateDevelopmentFn : createDevelopmentPlanFn;

		submitFunction(values);
	};

	const handleApproveFormSubmit = async ({
		confirmationDate,
	}: DevelopmentPlanApprovingFormValues) => {
		await updateDevelopmentPlanStatus({
			developmentPlanId: id!,
			developmentPlanStatus: getEnumKeyByValue(
				DevelopmentPlanStatus,
				DevelopmentPlanStatus.Approved
			),
			confirmationDate: dayjs(confirmationDate).format(
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			),
		});

		handleSuccessMutation();
	};

	const handleDeleteDevelopmentPlan = async (developmentPLanId: number) => {
		await deleteDevelopmentPlanMutation(developmentPLanId);
		handleSuccessMutation();
	};

	return {
		id,
		isOpen,
		mutationMode,
		selectedDevelopmentPlanStartDate,
		handleCloseClick,
		handleAddClick,
		handleEditOptionClick,
		handleApproveOptionClick,
		handleDeleteOptionClick,
		handleRejectOptionClick,
		handleAddOrEditFormSubmit,
		handleApproveFormSubmit,
		getDevelopmentPlanById,
		handleDeleteDevelopmentPlan,
		isDeleteLoading,
	};
};
