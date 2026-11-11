import { Button } from '@mms/mms-ui-library/components/Button';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useState } from 'react';

import { controlsText } from '@/constants/controlsTexts';

import { ButtonWrapper } from './styles';
import { PositiveActionButtonProps } from './types';

export function PositiveActionButton({
	disabled,
	tooltipText,
	onClick,
	text,
}: PositiveActionButtonProps) {
	const [isActionButtonHovered, setIsActionButtonHovered] = useState(false);

	const onMouseEnter = () => setIsActionButtonHovered(true);
	const onMouseLeave = () => setIsActionButtonHovered(false);

	if (tooltipText) {
		return (
			<Tooltip open={isActionButtonHovered && disabled} text={tooltipText}>
				<ButtonWrapper {...{ onMouseEnter, onMouseLeave }}>
					<Button {...{ disabled, onClick }} isFullWidth={false}>
						{text || controlsText.applyText}
					</Button>
				</ButtonWrapper>
			</Tooltip>
		);
	}

	return (
		<ButtonWrapper {...{ onMouseEnter, onMouseLeave }}>
			<Button {...{ disabled, onClick }} isFullWidth={false}>
				{text || controlsText.applyText}
			</Button>
		</ButtonWrapper>
	);
}
