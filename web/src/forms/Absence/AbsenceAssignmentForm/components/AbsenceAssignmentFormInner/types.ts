import { ChangeEvent, Dispatch, SetStateAction } from 'react';

import { getPopUpStylesConfig } from '../../styles';
import { AbsenceAssignmentFormValues, TransferListItem } from '../../types';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { Absence } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface AbsenceAssignmentFormInnerProps {
	formikProps: FormikWithBackupRenderProps<AbsenceAssignmentFormValues>;
	setSelectedAbsenceId: Dispatch<SetStateAction<number | null>>;
	setSelectedAgreementId: Dispatch<SetStateAction<number | null>>;
	absenceItems: Absence[];
	absencesOptions: SimpleConfigurableConfigurationItem<number>[];
	agreementOptions: SimpleConfigurableConfigurationItem<number>[];
	isAgreementMandatory: boolean;
	isLoading: boolean;
	isApplyDisabled: boolean;
	handleClose: VoidFunction;
	popUpStylesConfig: ReturnType<typeof getPopUpStylesConfig>;
	searchValue: string;
	handleClearSearch: VoidFunction;
	handleSearchChange: (e: ChangeEvent<HTMLInputElement>) => void;
	debouncedSearchValue: string;
	unassignedUsers: TransferListItem[];
	assignedUsers: TransferListItem[];
	setAssignedUsers: Dispatch<SetStateAction<TransferListItem[]>>;
	setUnassignedUsers: Dispatch<SetStateAction<TransferListItem[]>>;
	checkedUsers: TransferListItem[];
	setCheckedUsers: Dispatch<SetStateAction<TransferListItem[]>>;
}
