import { ADD_MORE_BUTTON_TITLE } from '@/constants/controlsTexts';

import { AddMoreButtonWrapper } from './styles';
import { AddMoreButtonProps } from './types';

export function AddMoreButton({
	handleClick,
	title = ADD_MORE_BUTTON_TITLE,
}: AddMoreButtonProps) {
	return (
		<AddMoreButtonWrapper type="button" onClick={handleClick} size="small">
			{title}
		</AddMoreButtonWrapper>
	);
}
