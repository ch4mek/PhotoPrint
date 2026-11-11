import { CSSProperties } from 'react';

import { WithMobileView } from '@/types/filter';

type FilterDropDownWithArrowsWidth = 'small' | 'large';
export interface FilterDropDownWithArrowsProps<T> extends WithMobileView {
	options: T[];
	value: T;
	onChange: (value: T) => void;
	onLeftArrowClick: () => void;
	onRightArrowClick: () => void;
	isLeftArrowDisabled?: boolean;
	isRightArrowDisabled?: boolean;
	getKey?: (option: T) => string | number;
	width?: FilterDropDownWithArrowsWidth;
	stylesConfig?: CSSProperties;
	fillAvailableWidthOnMobile?: boolean;
}

export interface FilterDropDownWithArrowsWrapperAttrs {
	width?: FilterDropDownWithArrowsWidth;
	fillAvailableWidthOnMobile?: boolean;
}
