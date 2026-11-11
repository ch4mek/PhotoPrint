import { DropDown } from '@mms/mms-ui-library/components/Dropdown';
import { ComponentProps } from 'react';

import { getFilterInputHeight } from '@/helpers/getFilterInputHeight';
import { WithMobileView } from '@/types/filter';

type FilterDropdownProps = ComponentProps<typeof DropDown> & WithMobileView;

export function FilterDropdown({
	isMobileView,
	...dropdownProps
}: FilterDropdownProps) {
	return (
		<DropDown height={getFilterInputHeight(isMobileView)} {...dropdownProps} />
	);
}
