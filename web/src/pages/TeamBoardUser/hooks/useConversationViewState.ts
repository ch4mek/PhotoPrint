import { useCallback, useRef, useState } from 'react';

import { ConversationRow } from '../types';

export const useConversationViewState = (rows: Array<ConversationRow>) => {
	const [viewConversation, setViewConversation] =
		useState<ConversationRow | null>(null);

	const rowsRef = useRef(rows);
	rowsRef.current = rows;

	const isViewOpen = viewConversation !== null;

	const handleViewClose = useCallback(() => {
		setViewConversation(null);
	}, []);

	const handleViewClick = useCallback(
		(conversationId?: string | number) => () => {
			const conversation = rowsRef.current.find(
				({ id }) => id === conversationId
			);

			if (conversation) {
				setViewConversation(conversation);
			}
		},
		[]
	);

	return {
		viewConversation,
		isViewOpen,
		handleViewClose,
		handleViewClick,
	};
};
