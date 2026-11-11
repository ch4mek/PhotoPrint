import { useEffect } from 'react';

import { useWikiContext } from '../../context/wikiContext';

import {
	HEADLINE_TITLE_WELCOME,
	HEADLINE_WIKI_TITLE,
	SUBHEADLINE_TITLE,
} from './constants';
import {
	HeadlineAccentTitle,
	HeadlineTitle,
	HeadlineWrapper,
	SubHeadlineTitle,
	WelcomeWikiLayoutWrapper,
} from './styles';

export function WelcomeWikiLayout() {
	const { isEditorReadOnly, setIsEditorReadOnly } = useWikiContext();

	useEffect(() => {
		if (!isEditorReadOnly) {
			setIsEditorReadOnly(true);
		}
	}, []);

	return (
		<WelcomeWikiLayoutWrapper>
			<HeadlineWrapper>
				<HeadlineTitle variant="large">
					{HEADLINE_TITLE_WELCOME}
					<HeadlineAccentTitle variant="large">
						{HEADLINE_WIKI_TITLE}
					</HeadlineAccentTitle>
				</HeadlineTitle>
				<SubHeadlineTitle variant="l-400">{SUBHEADLINE_TITLE}</SubHeadlineTitle>
			</HeadlineWrapper>
		</WelcomeWikiLayoutWrapper>
	);
}
