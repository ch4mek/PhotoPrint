import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useMemo } from 'react';

import { useResize } from '@/hooks/useResize';

import { ConversationViewPopUpContent } from './components/Content';
import { CONVERSATION_VIEW_POPUP_TITLE } from './config';
import { getPopupConfig } from './helpers/getPopupConfig';
import { ConversationViewPopUpProps } from './types';

export function ConversationViewPopUp({
	conversation,
	handleClose,
}: ConversationViewPopUpProps) {
	const theme = useTheme();
	const [, windowHeight] = useResize();

	const popUpStylesConfig = useMemo(
		() => getPopupConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	return (
		<PopUp
			type="info"
			title={CONVERSATION_VIEW_POPUP_TITLE}
			onClose={handleClose}
			showCloseButton
			hasPaddings={false}
			stylesConfig={popUpStylesConfig}
		>
			<ConversationViewPopUpContent conversation={conversation} />
		</PopUp>
	);
}
