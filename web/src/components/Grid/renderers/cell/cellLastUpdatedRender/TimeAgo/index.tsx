import TimeAgo from 'javascript-time-ago';
import en from 'javascript-time-ago/locale/en.json';
import { useMemo } from 'react';
import ReactTimeAgo from 'react-time-ago';

import type { TimeAgoComponentProps } from './types';

TimeAgo.addDefaultLocale(en);

export function TimeAgoComponent({ time }: TimeAgoComponentProps) {
	const memoizedTimeAgo = useMemo(() => new Date(time), [time]);

	return <ReactTimeAgo date={memoizedTimeAgo} locale="en-US" />;
}
