import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';

import { PopUpTitleHeader, PopUpTitleWrapper } from './styles';
import { PopUpTitleProps } from './types';

export function PopUpTitle({ headerText, extraContent }: PopUpTitleProps) {
	return (
		<PopUpTitleWrapper>
			<Tooltip
				text={headerText}
				useIfTextCut
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			>
				<PopUpTitleHeader Component="h3" variant="small">
					{headerText}
				</PopUpTitleHeader>
			</Tooltip>
			{extraContent}
		</PopUpTitleWrapper>
	);
}
