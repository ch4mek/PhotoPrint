import {
	Autoselect,
	AutoselectProps,
} from '@mms/mms-ui-library/components/Autoselects';

import { getFilterInputHeight } from '@/helpers/getFilterInputHeight';
import { WithMobileView } from '@/types/filter';

type FilterAutoselectProps<T> = AutoselectProps<T> & WithMobileView;

export function FilterAutoselect<T>({
	isMobileView,
	...dropdownProps
}: FilterAutoselectProps<T>) {
	return (
		<Autoselect
			height={getFilterInputHeight(isMobileView)}
			{...dropdownProps}
		/>
	);
}
