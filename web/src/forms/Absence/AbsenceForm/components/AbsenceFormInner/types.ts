import { Dispatch, RefObject, SetStateAction } from 'react';

import { AbsenceTabs } from '../../constants';
import { AbsenceFormValues } from '../../types';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface AbsenceFormInnerProps {
	formikProps: FormikWithBackupRenderProps<AbsenceFormValues>;
	setIsDetailsTabValid: Dispatch<SetStateAction<boolean>>;
	setIsNameExists: Dispatch<SetStateAction<boolean>>;
	checkIsNameExist: (name: string, id?: string | number) => boolean;
	dataId?: number | string | null;
	dataHasAnyTransactions?: boolean;
	absenceTypesOptions: SimpleConfigurableConfigurationItem[];
	activityTypesOptions: SimpleConfigurableConfigurationItem[];
	accrualTypeOptions: SimpleConfigurableConfigurationItem[];
	activeTab: AbsenceTabs;
	submitFormButtonRef: RefObject<HTMLButtonElement | null>;
}
