import { ChevronIcon } from '@mms/mms-ui-library/icons/base/ChevronIcon';

import { ChevronButton } from '@/components/ChevronButton';
import { EVENT_ENTER_KEY } from '@/constants/eventKeys';

import { CellExpanderFormatterWrapper } from './styles';
import { CellExpanderFormatterProps } from './types';

export function ExpandedCell({
	tabIndex,
	expanded,
	onCellExpand,
}: CellExpanderFormatterProps) {
	const handleKeyDown = (e: React.KeyboardEvent<HTMLSpanElement>) => {
		if (e.key === ' ' || e.key === EVENT_ENTER_KEY) {
			e.preventDefault();
			onCellExpand?.();
		}
	};

	return (
		<CellExpanderFormatterWrapper tabIndex={tabIndex}>
			<ChevronButton
				onClick={onCellExpand}
				onKeyDown={handleKeyDown}
				isActive={expanded}
			>
				<ChevronIcon />
			</ChevronButton>
		</CellExpanderFormatterWrapper>
	);
}
