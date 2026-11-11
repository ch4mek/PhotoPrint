import { Dispatch, SetStateAction, useCallback, useMemo } from 'react';

import { SalaryRow } from '../components/SalaryTable/types';
import { SalarySectionProps } from '../components/TeamBoardUserSummaryTab/SalarySection/types';
import { getNextIdInRows, mapUpdateSalaryResponseToSalaryRow } from '../utils';

import { DATE_FORMAT } from '@/constants/date';
import {
	NotificationType,
	USER_SALARY_UPDATED,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { StatusOfItem } from '@/forms/TeamboardForm/Conversation/types';
import {
	UpdateUserSalary,
	UserSalaryFormValues,
} from '@/forms/TeamboardForm/UserSalary/types';
import { createToast } from '@/helpers/createToast';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateUserSalary } from '@/queries/teamboard/useCreateUserSalary';
import { useUpdateUserSalary } from '@/queries/teamboard/useUpdateUserSalary';
import { TeamboardTableMode } from '@/types/teamboard';

const sortSalaryRowsDateByDesc = (rows: SalaryRow[]): SalaryRow[] =>
	rows.sort((a, b) => {
		const dateA = dayjs(a.salaryFrom, DATE_FORMAT);
		const dateB = dayjs(b.salaryFrom, DATE_FORMAT);

		return dateB.diff(dateA);
	});

interface UseSalaryFormStateProps extends SalarySectionProps {
	setRows?: Dispatch<SetStateAction<SalaryRow[]>>;
	rows?: SalaryRow[];
}

export const useSalaryFormState = ({
	mode,
	setUserSalaries,
	userSalaries,
	rows,
	setRows,
}: UseSalaryFormStateProps) => {
	const { handleOpen, isOpen, handleClose, id } = useShowModal<number>();
	const toast = useToast();

	const onMutationSuccess = () => {
		toast.open(createToast(NotificationType.SUCCESS, USER_SALARY_UPDATED));
		handleClose();
	};

	const { mutate: createUserSalaryMutation } = useCreateUserSalary({
		onSuccess: onMutationSuccess,
		onError: (error) => {
			if (error?.response?.data?.Message) {
				toast.open(
					createToast(NotificationType.ERROR, error.response.data.Message)
				);
			}
		},
	});

	const { mutate: updateUserSalaryMutation } = useUpdateUserSalary({
		onSuccess: onMutationSuccess,
		onError: (error) => {
			if (error?.response?.data?.Message) {
				toast.open(
					createToast(NotificationType.ERROR, error.response.data.Message)
				);
			}
		},
	});

	const handleSalaryEditClick = useCallback(
		(salaryId?: string | number) => () => {
			handleOpen(salaryId as number);
		},
		[]
	);

	const createSalaryQuery = (values: UpdateUserSalary) => {
		createUserSalaryMutation(values);
	};

	const updateSalaryQuery = (values: UpdateUserSalary) => {
		updateUserSalaryMutation(values);
	};

	const createSalary = (values: UpdateUserSalary) => {
		if (rows && setRows && setUserSalaries && userSalaries) {
			const dataToSave = {
				...values,
				id: getNextIdInRows(rows),
			};

			setRows(
				sortSalaryRowsDateByDesc([
					mapUpdateSalaryResponseToSalaryRow(dataToSave, handleSalaryEditClick),
					...rows,
				])
			);
			setUserSalaries([
				{ ...dataToSave, status: StatusOfItem.ADDED },
				...userSalaries,
			]);
			handleClose();
		}
	};

	const updateSalary = (values: UpdateUserSalary) => {
		if (rows && setRows && userSalaries && setUserSalaries) {
			const { id: idOfSalary } = values;

			setRows(
				rows.map((item) => {
					if (item.id === idOfSalary) {
						return mapUpdateSalaryResponseToSalaryRow(
							values,
							handleSalaryEditClick
						);
					}

					return item;
				})
			);

			if (userSalaries.every((userSalary) => userSalary.id !== idOfSalary)) {
				setUserSalaries([
					{ ...values, status: StatusOfItem.EDITED },
					...userSalaries,
				]);
			} else {
				setUserSalaries(
					userSalaries.map((item) => {
						if (item.id === idOfSalary) {
							return { ...values, status: StatusOfItem.ADDED };
						}

						return item;
					})
				);
			}
			handleClose();
		}
	};

	const getUserSalaryById = useMemo(
		() =>
			(idOfUserSalary: number): UserSalaryFormValues => {
				const { salaryFrom, salaryUsd } = rows?.find(
					(row) => row.id === idOfUserSalary
				) as SalaryRow;

				return {
					date: dayjs(salaryFrom, DATE_FORMAT),
					salaryUsd: salaryUsd.slice(0, -1),
				};
			},
		[rows]
	);

	const handleFormSubmit = (values: UpdateUserSalary) =>
		new Promise((resolve) => {
			const updateSalaryFn =
				mode === TeamboardTableMode.PopUp ? updateSalary : updateSalaryQuery;
			const createSalaryFn =
				mode === TeamboardTableMode.PopUp ? createSalary : createSalaryQuery;

			const submitFunction = id ? updateSalaryFn : createSalaryFn;

			resolve(submitFunction(values));
		});

	const handleAddClick = useCallback(() => {
		handleOpen();
	}, []);

	return {
		id,
		isOpen,
		handleClose,
		handleFormSubmit,
		handleSalaryEditClick,
		handleAddClick,
		getUserSalaryById,
	};
};
