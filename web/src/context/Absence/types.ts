import { Dispatch, SetStateAction } from 'react';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface AbsenceHistoryContextValues {
	yearFilter: SelectionModalOption;
	absenceFilter: SimpleConfigurableConfigurationItem;
	setYearFilter: Dispatch<SetStateAction<SelectionModalOption>>;
	setAbsenceFilter: Dispatch<
		SetStateAction<SimpleConfigurableConfigurationItem>
	>;
}
