import { getTextSections } from '../../config';
import { PropsWithConversationData } from '../../types';
import { Details } from '../Details';
import { Documents } from '../Documents';
import { FieldSection } from '../FieldSection';

export function ConversationViewPopUpContent({
	conversation,
}: PropsWithConversationData) {
	const sections = getTextSections(conversation);

	return (
		<>
			<Details conversation={conversation} />
			{sections.map(({ key, label, value }) => (
				<FieldSection key={key} label={label} value={value} />
			))}
			<Documents documents={conversation.files} />
		</>
	);
}
