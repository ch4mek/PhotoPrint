import { useTheme } from '@emotion/react';
import { FormikHelpers } from 'formik';
import {
	ChangeEvent,
	useCallback,
	useMemo,
	useState,
	useEffect,
	useRef,
} from 'react';

import {
	ASSIGNMENT_FORM_TITLE,
	initialFormValues,
	SEARCH_INPUT_DELAY,
} from '@/forms/Absence/AbsenceAssignmentForm/constants';
import { getValidationSchema } from '@/forms/Absence/AbsenceAssignmentForm/schema';
import { getPopUpStylesConfig } from '@/forms/Absence/AbsenceAssignmentForm/styles';
import {
	AbsenceAssignmentFormProps,
	AbsenceAssignmentFormValues,
	TransferListItem,
} from '@/forms/Absence/AbsenceAssignmentForm/types';
import { getAbsencesOptions } from '@/forms/Absence/AbsenceAssignmentForm/utils/getAbsencesOptions';
import { getAgreementOptions } from '@/forms/Absence/AbsenceAssignmentForm/utils/getAgreementOptions';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { useDebounce } from '@/hooks/useDebounce';
import { useResize } from '@/hooks/useResize';
import { useGetAbsencesData } from '@/pages/Settings/components/AbsencesTab/hooks/useGetAbsencesData';
import { AbsenceType } from '@/types/absences';

import { AbsenceAssignmentFormInner } from './components/AbsenceAssignmentFormInner';
import { differenceById } from './components/TransferList/utils/differenceById';
import { useGetUsersForAssignmentsData } from './hooks/useGetUsersForAssignmentsData';
import { convertAssignmentDataToRequest } from './utils/convertAssignmentDataToRequest';
import { convertResponseToTransferListData } from './utils/convertResponseToTransferListData';

export function AbsenceAssignmentForm({
	handleClose,
	handleFormSubmit,
	isLoading,
}: AbsenceAssignmentFormProps) {
	const theme = useTheme();
	const [windowWidth, windowHeight] = useResize();
	const [selectedAbsenceId, setSelectedAbsenceId] = useState<number | null>(
		null
	);
	const [selectedAgreementId, setSelectedAgreementId] = useState<number | null>(
		null
	);

	const { absenceItems, isLoading: isAbsenceLoading } = useGetAbsencesData();

	const [searchValue, setSearchValue] = useState('');
	const debouncedSearchValue = useDebounce(searchValue, SEARCH_INPUT_DELAY);

	const [assignedUsers, setAssignedUsers] = useState<TransferListItem[]>([]);
	const [unassignedUsers, setUnassignedUsers] = useState<TransferListItem[]>(
		[]
	);
	const [checkedUsers, setCheckedUsers] = useState<TransferListItem[]>([]);
	const initialAssignedUsersRef = useRef<TransferListItem[]>([]);
	const initialUnassignedUsersRef = useRef<TransferListItem[]>([]);

	const absencesOptions = useMemo(
		() => getAbsencesOptions(absenceItems),
		[absenceItems]
	);
	const agreementOptions = useMemo(
		() => getAgreementOptions(selectedAbsenceId, absenceItems),
		[absenceItems, selectedAbsenceId]
	);

	const { isLoading: isUsersLoading, refetch } = useGetUsersForAssignmentsData(
		selectedAbsenceId,
		selectedAgreementId
	);

	const isAgreementMandatory = useMemo(() => {
		const selectedAbsence = absenceItems.find(
			({ id }) => selectedAbsenceId === id
		);

		if (!selectedAbsence) {
			return false;
		}

		const { type } = selectedAbsence;

		return type === AbsenceType.Vacation || type === AbsenceType.SickDay;
	}, [selectedAbsenceId, absenceItems]);

	const isUsersFetchingEnabled =
		selectedAbsenceId && (!isAgreementMandatory || selectedAgreementId);

	useEffect(() => {
		if (isUsersFetchingEnabled) {
			refetch().then(({ data }) => {
				const assignedUsersResponse = convertResponseToTransferListData(
					data?.assignedUsers
				);
				const unassignedUsersResponse = convertResponseToTransferListData(
					data?.unassignedUsers
				);

				initialAssignedUsersRef.current = assignedUsersResponse;
				initialUnassignedUsersRef.current = unassignedUsersResponse;

				setCheckedUsers([]);
				setAssignedUsers(assignedUsersResponse);
				setUnassignedUsers(unassignedUsersResponse);
			});
		} else {
			initialAssignedUsersRef.current = [];
			initialUnassignedUsersRef.current = [];

			setCheckedUsers([]);
			setAssignedUsers([]);
			setUnassignedUsers([]);
		}
	}, [isUsersFetchingEnabled, selectedAbsenceId, selectedAgreementId]);

	const isApplyDisabled = useMemo(
		() =>
			!differenceById(assignedUsers, initialAssignedUsersRef.current).length &&
			!differenceById(unassignedUsers, initialUnassignedUsersRef.current)
				.length,
		[assignedUsers, unassignedUsers]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowWidth, windowHeight),
		[theme, windowWidth, windowHeight]
	);

	const validationSchema = useMemo(
		() => getValidationSchema(isAgreementMandatory),
		[agreementOptions]
	);

	const handleSubmit = useCallback(
		async (
			values: AbsenceAssignmentFormValues,
			{ setSubmitting }: FormikHelpers<AbsenceAssignmentFormValues>
		) => {
			try {
				setSubmitting(true);
				const data = convertAssignmentDataToRequest(
					values,
					initialUnassignedUsersRef.current,
					initialAssignedUsersRef.current,
					unassignedUsers,
					assignedUsers
				);
				await handleFormSubmit(data);
			} finally {
				setSubmitting(false);
			}
		},
		[unassignedUsers, assignedUsers]
	);

	const handleSearchChange = useCallback((e: ChangeEvent<HTMLInputElement>) => {
		setSearchValue(e.target.value);
	}, []);

	const handleClearSearch = useCallback(() => {
		setSearchValue('');
	}, []);

	return (
		<FormikWithBackup
			initialValues={initialFormValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			enableReinitialize
			validateOnBlur={false}
			name={ASSIGNMENT_FORM_TITLE}
		>
			{(formikProps) => (
				<AbsenceAssignmentFormInner
					{...{
						formikProps,
						setSelectedAbsenceId,
						setSelectedAgreementId,
						absenceItems,
						absencesOptions,
						agreementOptions,
						isAgreementMandatory,
						isApplyDisabled,
						handleClose,
						popUpStylesConfig,
						searchValue,
						handleClearSearch,
						handleSearchChange,
						debouncedSearchValue,
						unassignedUsers,
						assignedUsers,
						setAssignedUsers,
						setUnassignedUsers,
						checkedUsers,
						setCheckedUsers,
					}}
					isLoading={isLoading || isAbsenceLoading || isUsersLoading}
				/>
			)}
		</FormikWithBackup>
	);
}
