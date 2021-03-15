import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ConfirmationService, MessageService } from "primeng/api";
import { Table } from "primeng/table";
import { Courses } from "src/app/models/course";
import { CoursesService } from "../../services/courses.service";

@Component({
	selector: "app-courses",
	templateUrl: "./courses.component.html",
	styleUrls: ["./courses.component.scss"],
})
export class CoursesComponent implements OnInit {
	course: Courses;
	courses: Courses[] = [];
	courseDialog: boolean;
	submitted: boolean;
	rows: number = 10;
	courseForm: FormGroup;

	constructor(
		private service: CoursesService,
		private messageService: MessageService,
		private confirmationService: ConfirmationService,
		private fb: FormBuilder
	) {}

	ngOnInit() {
		this.courseForm = this.fb.group({});
		this.service.getCourses().subscribe((res) => (this.courses = res));
	}
	create() {
		this.course = {};
		this.courseDialog = true;
		this.submitted = false;
	}
	hideDialog() {
		this.courseDialog = false;
		this.submitted = false;
	}
	saveCourses() {
		this.submitted = true;
		if (this.course.title.trim()) {
			if (this.course.id) {
				this.courses[this.findIndexById(this.course.id)] = this.course;
				this.service
					.updateCourse(this.course.id, this.course)
					.subscribe(() =>
						this.messageService.add({
							severity: "success",
							summary: "Successful",
							detail: "Course updated successfully",
							life: 3000,
						})
					);
			} else {
				this.service.createCourse(this.course).subscribe((res) => {
					this.messageService.add({
						severity: "success",
						summary: "Successful",
						detail: "Course Created",
						life: 3000,
					});
					this.courses.push(res);
				});
			}
			this.courseDialog = false;
			this.courses = [...this.courses];
			this.course = {};
		}
	}
	editCourse(id: string, course: Courses) {
		this.course = { id: id, ...course };
		this.courseDialog = true;
	}
	deleteCourse(course: Courses) {
		this.confirmationService.confirm({
			message: "Are you sure you want to delete " + course.title + "?",
			header: "Confirm",
			icon: "pi pi-exclamation-triangle",
			accept: () => {
				this.service.deleteCourse(course.id).subscribe(() => {
					this.courses = this.courses.filter(
						(val) => val.id !== course.id
					);
					this.course = {};
					this.messageService.add({
						severity: "success",
						summary: "Successful",
						detail: "Course deleted successfully",
						life: 3000,
					});
				});
			},
		});
	}
	onSearch(dt: Table, event: any) {
		dt.filterGlobal(event.target.value, "contains");
	}
	findIndexById(id: string): number {
		let index = -1;
		for (let i = 0; i < this.courses.length; i++) {
			if (this.courses[i].id === id) {
				index = i;
				break;
			}
		}
		return index;
	}
}
