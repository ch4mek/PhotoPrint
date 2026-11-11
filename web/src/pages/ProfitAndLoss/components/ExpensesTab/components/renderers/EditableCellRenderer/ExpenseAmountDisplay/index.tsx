import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';

import {
	ExpenseAmountDisplayContent,
	ExpenseAmountDisplayWrapper,
} from './styles';
import { ExpenseAmountDisplayProps } from './types';

export function ExpenseAmountDisplay({ value }: ExpenseAmountDisplayProps) {
	return (
		<ExpenseAmountDisplayWrapper>
			{value && (
				<Tooltip
					zIndex={zIndexes.gridTooltip}
					text={String(value)}
					position="bottom"
					fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
					useIfTextCut
				>
					<ExpenseAmountDisplayContent>{value}</ExpenseAmountDisplayContent>
				</Tooltip>
			)}
		</ExpenseAmountDisplayWrapper>
	);
}
