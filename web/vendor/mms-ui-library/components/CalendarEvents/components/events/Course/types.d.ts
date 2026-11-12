import { BaseEventProps } from '../types';
export declare enum CourseStatus {
    Start = "start",
    End = "end"
}
export type CourseProps = BaseEventProps & {
    courseStatus: CourseStatus;
    onCourseClick?: (id: string) => void;
};
