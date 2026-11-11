import { useTheme } from '@emotion/react';
import { RenderCellProps } from 'react-data-grid';

import { ExpandableGridCellProps } from '../../types';

import { ChevronButton } from '@/components/ChevronButton';

import { ExpandCellWrapper, ExpandChevronButtonWrapper } from './styles';
import { ExpandCellRendererOptions } from './types';
import {
	defaultGetExpanded,
	defaultGetHasChildren,
	defaultGetIsDisabled,
	defaultOnExpandChange,
	defaultShouldRender,
} from './utils';

export const ExpandCellRenderer = <T extends ExpandableGridCellProps>(
	{ row, onRowChange }: RenderCellProps<T, unknown>,
	{
		shouldRender = defaultShouldRender,
		getHasChildren = defaultGetHasChildren,
		getExpanded = defaultGetExpanded,
		getIsDisabled = defaultGetIsDisabled,
		onExpandChange = defaultOnExpandChange,
		chevronColor,
		chevronSize = 'small',
	}: ExpandCellRendererOptions<T> = {}
) => {
	const theme = useTheme();

	if (!shouldRender(row)) {
		return null;
	}

	const hasChildren = getHasChildren(row);
	const expanded = getExpanded(row);
	const isDisabled = getIsDisabled(row, hasChildren);

	const handleExpand = () => {
		if (!isDisabled && onRowChange) {
			onRowChange(onExpandChange(row, !expanded));
		}
	};

	return (
		<ExpandCellWrapper onClick={handleExpand} disabled={isDisabled}>
			<ExpandChevronButtonWrapper disabled={isDisabled}>
				<ChevronButton
					chevronColor={chevronColor ?? theme.palette['graphics-light-gray']}
					isActive={expanded}
					disabled={isDisabled}
					size={chevronSize}
				/>
			</ExpandChevronButtonWrapper>
		</ExpandCellWrapper>
	);
};
