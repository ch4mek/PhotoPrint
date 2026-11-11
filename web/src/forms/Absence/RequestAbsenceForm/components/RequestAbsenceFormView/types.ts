import { Holiday } from '@mms/mms-ui-library/components/CustomCalendarSingle';
import { Dispatch, SetStateAction } from 'react';

import { useRequestDocuments } from '../../hooks/useRequestDocuments';
import { InitialDataValues } from '../../types';
import { getPopUpStylesConfig } from '../../utils/getPopUpStylesConfig';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { RequestSelectedParam } from '@/queries/inquiries/types';
import { UserAbsenceItem } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface RequestAbsenceFormViewProps {
	formikProps: FormikWithBackupRenderProps<InitialDataValues>;
	selectedAbsence: UserAbsenceItem | null;
	absences: UserAbsenceItem[] | undefined;
	absenceNamesOptions: SimpleConfigurableConfigurationItem<string | number>[];
	holidays: Holiday[];
	isDisableForm: boolean;
	isShowLoader: boolean;
	popUpStylesConfig: ReturnType<typeof getPopUpStylesConfig>;
	documentHandlers: ReturnType<typeof useRequestDocuments>;
	selectedDays: number | null;
	handleClose: () => void;
	getRequestSelectedDays: (filter: RequestSelectedParam) => void;
	setSelectedAbsence: Dispatch<SetStateAction<UserAbsenceItem | null>>;
	userId?: number;
}
