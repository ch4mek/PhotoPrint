import { normalizeIsoDate } from '@/helpers/normalizeIsoDate';
import { Message } from '@/types/chatWidget';

export const groupMessagesByDate = (messages: Message[]) => {
	const sorted = [...messages].sort((a, b) =>
		normalizeIsoDate(a.createdAtUtc).localeCompare(
			normalizeIsoDate(b.createdAtUtc)
		)
	);

	return sorted.reduce<Record<string, Message[]>>((acc, m) => {
		const key = m.createdAtUtc.slice(0, 10);
		(acc[key] ??= []).push(m);

		return acc;
	}, {});
};
